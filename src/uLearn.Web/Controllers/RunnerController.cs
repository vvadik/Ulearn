using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Database;
using Database.DataContexts;
using Database.Models;
using log4net;
using RunCsJob.Api;
using Telegram.Bot.Types.Enums;
using uLearn.Telegram;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using XQueue;
using XQueue.Models;

namespace uLearn.Web.Controllers
{
	public class RunnerController : ApiController
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(RunnerController));

		private readonly UserSolutionsRepo userSolutionsRepo;
		private readonly CourseManager courseManager;

		private static readonly List<IResultObserver> resultObserveres = new List<IResultObserver>
		{
			new XQueueResultObserver(),
			new SandboxErrorsResultObserver(),
		};

		public RunnerController(ULearnDb db, CourseManager courseManager)
		{
			this.courseManager = courseManager;
			userSolutionsRepo = new UserSolutionsRepo(db, courseManager);
		}

		public RunnerController()
			: this(new ULearnDb(), WebCourseManager.Instance)
		{
		}

		[HttpGet]
		[Route("GetSubmissions")]
		public async Task<List<RunnerSubmission>> GetSubmissions([FromUri] string token, [FromUri] int count)
		{
			CheckRunner(token);
			var sw = Stopwatch.StartNew();
			while (true)
			{
				var repo = new UserSolutionsRepo(new ULearnDb(), courseManager);
				var submissions = await repo.GetUnhandledSubmissions(count);
				if (submissions.Any() || sw.Elapsed > TimeSpan.FromSeconds(15))
				{
					if (submissions.Any())
						log.Info($"Отдаю на проверку решения: [{string.Join(",", submissions.Select(c => c.Id))}]");
					return submissions.Select(ToRunnerSubmission).ToList();
				}
				await repo.WaitAnyUnhandledSubmissions(TimeSpan.FromSeconds(10));
			}
		}

		private RunnerSubmission ToRunnerSubmission(UserExerciseSubmission submission)
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
			var exerciseSlide = courseManager.FindCourse(submission.CourseId)?.FindSlideById(submission.SlideId) as ExerciseSlide;
			if (exerciseSlide == null)
				return new FileRunnerSubmission
				{
					Id = submission.Id.ToString(),
					Code = "// no slide anymore",
					Input = "",
					NeedRun = true
				};

			courseManager.WaitWhileCourseIsLocked(submission.CourseId);

			return exerciseSlide.Exercise.CreateSubmission(
				submission.Id.ToString(),
				submission.SolutionCode.Text
			);
		}

		[HttpPost]
		[Route("PostResults")]
		public async Task PostResults([FromUri] string token, List<RunningResults> results)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
				log.Error($"Не могу принять от RunCsJob результаты проверки решений, ошибки: {string.Join(", ", errors)}");
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}
			CheckRunner(token);
			log.Info($"Получил от RunCsJob результаты проверки решений: [{string.Join(", ", results.Select(r => r.Id))}]");
			await FuncUtils.TrySeveralTimesAsync(() => userSolutionsRepo.SaveResults(results), 3);

			var submissionsByIds = userSolutionsRepo
				.FindSubmissionsByIds(results.Select(result => result.Id).ToList())
				.ToDictionary(s => s.Id.ToString());

			foreach (var result in results)
			{
				if (!submissionsByIds.ContainsKey(result.Id))
					continue;
				await SendResultToObservers(submissionsByIds[result.Id], result);
			}
		}

		private Task SendResultToObservers(UserExerciseSubmission submission, RunningResults result)
		{
			var tasks = resultObserveres.Select(o => o.ProcessResult(submission, result));
			return Task.WhenAll(tasks);
		}

		private void CheckRunner(string token)
		{
			var expectedToken = ConfigurationManager.AppSettings["runnerToken"];
			if (expectedToken != token)
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
		}
	}

	public interface IResultObserver
	{
		Task ProcessResult(UserExerciseSubmission submission, RunningResults result);
	}

	public class XQueueResultObserver : IResultObserver
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(XQueueResultObserver));

		public async Task ProcessResult(UserExerciseSubmission submission, RunningResults result)
		{
			var courseManager = WebCourseManager.Instance;

			var xQueueRepo = new XQueueRepo(new ULearnDb(), courseManager);
			var xQueueSubmission = xQueueRepo.FindXQueueSubmission(submission);
			if (xQueueSubmission == null)
				return;
				
			var watcher = xQueueSubmission.Watcher;
			var client = new XQueueClient(watcher.BaseUrl, watcher.UserName, watcher.Password);
			await client.Login();
			if (await SendSubmissionResultsToQueue(client, xQueueSubmission))
				await xQueueRepo.MarkXQueueSubmissionThatResultIsSent(xQueueSubmission);
		}

		private async Task<bool> SendSubmissionResultsToQueue(XQueueClient client, XQueueExerciseSubmission submission)
		{
			return await FuncUtils.TrySeveralTimesAsync(() => TrySendSubmissionResultsToQueue(client, submission), 5, () => Task.Delay(TimeSpan.FromMilliseconds(1)));
		}

		private async Task<bool> TrySendSubmissionResultsToQueue(XQueueClient client, XQueueExerciseSubmission submission)
		{
			var checking = submission.Submission.AutomaticChecking;

			var courseManager = WebCourseManager.Instance;
			var slide = courseManager.FindCourse(checking.CourseId)?.FindSlideById(checking.SlideId) as ExerciseSlide;
			if (slide == null)
			{
				log.Warn($"Can't find exercise slide {checking.SlideId} in course {checking.CourseId}. Exit");
				return false;
			}

			var score = (double)checking.Score / slide.Exercise.CorrectnessScore;
			if (score > 1)
				score = 1;

			var message = checking.IsCompilationError ? checking.CompilationError.Text : checking.Output.Text;
			return await client.PutResult(new XQueueResult
			{
				header = submission.XQueueHeader,
				Body = new XQueueResultBody
				{
					IsCorrect = checking.IsRightAnswer,
					Message = message,
					Score = score,
				}
			});
		}
	}

	public class SandboxErrorsResultObserver : IResultObserver
	{
		private static readonly ErrorsBot bot = new ErrorsBot();

		public async Task ProcessResult(UserExerciseSubmission submission, RunningResults result)
		{
			/* Ignore all verdicts except SandboxError */
			if (result.Verdict != Verdict.SandboxError)
				return;

			var output = result.Output;
			await bot.PostToChannelAsync(
				$"<b>Решение #{submission.Id} не запустилось в песочнице (SandboxError).</b>\n" +
				(string.IsNullOrEmpty(output) ? "" : $"Вывод:\n<pre>{output.EscapeHtml()}</pre>"), 
				ParseMode.Html
			);
		}
	}
}