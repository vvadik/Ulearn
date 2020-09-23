using System;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Serilog;
using Ulearn.Common;
using Ulearn.Core.Courses.Slides.Exercises;
using Ulearn.Core.RunCheckerJobApi;
using XQueue;
using XQueue.Models;

namespace Ulearn.Web.Api.Controllers.Runner
{
	public class XQueueResultObserver : IResultObserver
	{
		private readonly ILogger logger;
		private readonly IWebCourseManager courseManager;
		private readonly XQueueRepo xQueueRepo;

		public XQueueResultObserver(ILogger logger, IWebCourseManager courseManager, XQueueRepo xQueueRepo)
		{
			this.logger = logger;
			this.courseManager = courseManager;
			this.xQueueRepo = xQueueRepo;
		}

		public async Task ProcessResult(UserExerciseSubmission submission, RunningResults result)
		{
			var xQueueSubmission = await xQueueRepo.FindXQueueSubmission(submission);
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
			return await FuncUtils.TrySeveralTimesAsync(
				() => TrySendSubmissionResultsToQueue(client, submission),
				5, 
				() => Task.Delay(TimeSpan.FromMilliseconds(1)));
		}

		private async Task<bool> TrySendSubmissionResultsToQueue(XQueueClient client, XQueueExerciseSubmission submission)
		{
			var checking = submission.Submission.AutomaticChecking;

			var slide = (await courseManager.FindCourseAsync(checking.CourseId))?.FindSlideById(checking.SlideId, true) as ExerciseSlide;
			if (slide == null)
			{
				logger.Warning($"Can't find exercise slide {checking.SlideId} in course {checking.CourseId}. Exit");
				return false;
			}

			var score = (double)checking.Score / slide.Scoring.PassedTestsScore;
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
}