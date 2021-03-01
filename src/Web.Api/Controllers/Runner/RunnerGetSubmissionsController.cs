using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Vostok.Logging.Abstractions;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.Core.Courses.Slides.Exercises;
using Ulearn.Core.Courses.Slides.Exercises.Blocks;
using Ulearn.Core.RunCheckerJobApi;
using Web.Api.Configuration;

namespace Ulearn.Web.Api.Controllers.Runner
{
	public class RunnerGetSubmissionsController : BaseController
	{
		private readonly IUserSolutionsRepo userSolutionsRepo;
		private readonly WebApiConfiguration configuration;
		private static ILog log => LogProvider.Get().ForContext(typeof(RunnerGetSubmissionsController));

		public RunnerGetSubmissionsController(IWebCourseManager courseManager, UlearnDb db, IOptions<WebApiConfiguration> options,
			IUsersRepo usersRepo, IUserSolutionsRepo userSolutionsRepo)
			: base(courseManager, db, usersRepo)
		{
			configuration = options.Value;
			this.userSolutionsRepo = userSolutionsRepo;
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

			var sw = Stopwatch.StartNew();
			while (true)
			{
				var submission = await userSolutionsRepo.GetUnhandledSubmission(agent, sandboxesImageNames).ConfigureAwait(false);
				if (submission != null || sw.Elapsed > TimeSpan.FromSeconds(10))
				{
					if (submission != null)
						log.Info($"Отдаю на проверку решение: [{submission.Id}], агент {agent}, только сначала соберу их");
					else
						return new List<RunnerSubmission>();

					var builtSubmissions = new List<RunnerSubmission> { await ToRunnerSubmission(submission) };
					log.Info($"Собрал решения: [{submission.Id}], отдаю их агенту {agent}");
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

			log.Info($"Собираю для отправки в RunCsJob решение {submission.Id}");
			var slide = (await courseManager.FindCourseAsync(submission.CourseId))?.FindSlideById(submission.SlideId, true);

			if (slide is ExerciseSlide exerciseSlide)
			{
				log.Info($"Ожидаю, если курс {submission.CourseId} заблокирован");
				courseManager.WaitWhileCourseIsLocked(submission.CourseId);
				log.Info($"Курс {submission.CourseId} разблокирован");

				if (exerciseSlide is PolygonExerciseSlide)
					return ((PolygonExerciseBlock)exerciseSlide.Exercise).CreateSubmission(
						submission.Id.ToString(),
						submission.SolutionCode.Text,
						submission.Language
					);

				return exerciseSlide.Exercise.CreateSubmission(
					submission.Id.ToString(),
					submission.SolutionCode.Text
				);
			}

			return new FileRunnerSubmission
			{
				Id = submission.Id.ToString(),
				Code = "// no slide anymore",
				Input = "",
				NeedRun = true
			};
		}
	}
}