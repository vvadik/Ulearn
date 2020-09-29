using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.Groups;
using Database.Repos.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using Ulearn.Common;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.Core.Courses.Slides.Exercises;
using Ulearn.Core.Metrics;
using Ulearn.Core.RunCheckerJobApi;
using Ulearn.Web.Api.Controllers.Slides;
using Web.Api.Configuration;

namespace Ulearn.Web.Api.Controllers.Runner
{
	public class RunnerController : BaseController
	{
		private readonly IUserSolutionsRepo userSolutionsRepo;
		private readonly ISlideCheckingsRepo slideCheckingsRepo;
		private readonly IGroupsRepo groupsRepo;
		private readonly IVisitsRepo visitsRepo;
		private readonly MetricSender metricSender;
		private readonly WebApiConfiguration configuration;

		private readonly List<IResultObserver> resultObservers;

		public RunnerController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IOptions<WebApiConfiguration> options,
			IUsersRepo usersRepo, IUserSolutionsRepo userSolutionsRepo, ISlideCheckingsRepo slideCheckingsRepo,
			IGroupsRepo groupsRepo, IVisitsRepo visitsRepo, MetricSender metricSender,
			XQueueResultObserver xQueueResultObserver, SandboxErrorsResultObserver sandboxErrorsResultObserver,
			AntiPlagiarismResultObserver antiPlagiarismResultObserver, StyleErrorsResultObserver styleErrorsResultObserver)
			: base(logger, courseManager, db, usersRepo)
		{
			configuration = options.Value;
			resultObservers = new List<IResultObserver>
			{
				xQueueResultObserver,
				sandboxErrorsResultObserver,
				antiPlagiarismResultObserver,
				styleErrorsResultObserver,
			};
			this.userSolutionsRepo = userSolutionsRepo;
			this.slideCheckingsRepo = slideCheckingsRepo;
			this.groupsRepo = groupsRepo;
			this.visitsRepo = visitsRepo;
			this.metricSender = metricSender;
		}

		/// <summary>
		/// Взять на проверку решения задач
		/// </summary>
		[HttpPost("/runner/get-submissions")]
		public async Task<ActionResult<List<RunnerSubmission>>> GetSubmissions([FromQuery] string token, [FromQuery] string sandboxes, [FromQuery] string agent)
		{
			if (configuration.RunnerToken != token)
				return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResponse("Invalid token"));

			var sandboxesImageNames = sandboxes.Split(',').ToList();
			if (sandboxesImageNames.Contains("csharp"))
				sandboxesImageNames.Add(null);

			var sw = Stopwatch.StartNew();
			while (true)
			{
				var submission = await userSolutionsRepo.GetUnhandledSubmission(agent, sandboxesImageNames).ConfigureAwait(false);
				if (submission != null || sw.Elapsed > TimeSpan.FromSeconds(10))
				{
					if (submission != null)
						logger.Information($"Отдаю на проверку решение: [{submission.Id}], агент {agent}, только сначала соберу их");
					else
						return new List<RunnerSubmission>();

					var builtSubmissions = new List<RunnerSubmission> { await ToRunnerSubmission(submission) };
					logger.Information($"Собрал решения: [{submission.Id}], отдаю их агенту {agent}");
					return builtSubmissions;
				}

				await Task.Delay(TimeSpan.FromMilliseconds(50));
				await userSolutionsRepo.WaitAnyUnhandledSubmissions(TimeSpan.FromSeconds(8)).ConfigureAwait(false);
			}
		}

		private async Task<RunnerSubmission> ToRunnerSubmission(UserExerciseSubmission submission)
		{
			if (submission.IsWebSubmission)
			{
				return new FileRunnerSubmission
				{
					Id = submission.Id.ToString(),
					Code = submission.SolutionCode.Text,
					Input = "",
					NeedRun = true
				};
			}

			logger.Information($"Собираю для отправки в RunCsJob решение {submission.Id}");

			var exerciseSlide = (await courseManager.FindCourseAsync(submission.CourseId))?.FindSlideById(submission.SlideId, true) as ExerciseSlide;
			if (exerciseSlide == null)
				return new FileRunnerSubmission
				{
					Id = submission.Id.ToString(),
					Code = "// no slide anymore",
					Input = "",
					NeedRun = true
				};

			logger.Information($"Ожидаю, если курс {submission.CourseId} заблокирован");
			courseManager.WaitWhileCourseIsLocked(submission.CourseId);
			logger.Information($"Курс {submission.CourseId} разблокирован");

			return exerciseSlide.Exercise.CreateSubmission(
				submission.Id.ToString(),
				submission.SolutionCode.Text
			);
		}

		/// <summary>
		/// Записать результат проверки решений задач
		/// </summary>
		[HttpPost("/runner/set-result")]
		public async Task<ActionResult> SetResults([FromQuery] string token, [FromQuery] string agent, [FromBody] List<RunningResults> results)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
				var errorStr = $"Не могу принять от RunCsJob результаты проверки решений, ошибки: {string.Join(", ", errors)}";
				logger.Error(errorStr);
				return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResponse(errorStr));
			}

			if (configuration.RunnerToken != token)
				return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResponse("Invalid token"));
			logger.Information($"Получил от RunCsJob результаты проверки решений: [{string.Join(", ", results.Select(r => r.Id))}] от агента {agent}");

			foreach (var result in results)
				await FuncUtils.TrySeveralTimesAsync(() => userSolutionsRepo.SaveResult(result,
					submission => ExerciseController.SendToReviewAndUpdateScore(submission, courseManager, slideCheckingsRepo, groupsRepo, visitsRepo, metricSender)
				), 3).ConfigureAwait(false);

			var submissionsByIds = (await userSolutionsRepo
				.FindSubmissionsByIds(results
					.Select(result => int.TryParse(result.Id, out var parsed) ? parsed : -1)
					.Where(i => i != -1)
					.Distinct()
					.ToList())
				).ToDictionary(s => s.Id.ToString());

			foreach (var result in results)
			{
				if (!submissionsByIds.ContainsKey(result.Id))
					continue;
				await SendResultToObservers(submissionsByIds[result.Id], result).ConfigureAwait(false);
			}

			return StatusCode((int)HttpStatusCode.Accepted);
		}

		private Task SendResultToObservers(UserExerciseSubmission submission, RunningResults result)
		{
			var tasks = resultObservers.Select(o => o.ProcessResult(submission, result));
			return Task.WhenAll(tasks);
		}
	}
}