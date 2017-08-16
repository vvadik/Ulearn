using System;
using System.Threading.Tasks;
using Database.DataContexts;
using Database.Models;
using log4net;
using uLearn;
using XQueue;
using XQueue.Models;

namespace XQueueWatcher
{
	public class Watcher
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Watcher));

		public readonly Database.Models.XQueueWatcher watcher;
		private readonly CourseManager courseManager;
		private readonly XQueueClient client;

		public EventHandler<SubmissionEventArgs> OnSubmission;

		public Watcher(Database.Models.XQueueWatcher watcher, CourseManager courseManager)
		{
			client = new XQueueClient(watcher.BaseUrl, watcher.UserName, watcher.Password);
			this.watcher = watcher;
			this.courseManager = courseManager;
		}

		public async Task Loop()
		{
			if (!await client.Login())
			{
				log.Error("Can\'t login to xqueue. Stop watcher");
				return;
			}

			while (true)
			{
				try
				{
					await OneStep();
				}
				catch (Exception e)
				{
					log.Error($"Exception occurred while doing one step in the loop: {e.Message}", e);
				}
				await Task.Delay(TimeSpan.FromSeconds(1));
			}
		}

		private async Task OneStep()
		{
			await ProcessOneSubmissionFromQueue();
			await SendResultsToQueue();
		}

		private async Task SendResultsToQueue()
		{
			var db = new ULearnDb();
			var xQueueRepo = new XQueueRepo(db, courseManager);
			var submissionsReadyToSentResults = xQueueRepo.GetXQueueSubmissionsReadyToSentResults(watcher);
			foreach (var submission in submissionsReadyToSentResults)
			{
				if (await SendSubmissionResultsToQueue(submission))
					await xQueueRepo.MarkXQueueSubmissionThatResultIsSent(submission);
			}
		}

		private async Task<bool> SendSubmissionResultsToQueue(XQueueExerciseSubmission submission)
		{
			return await FuncUtils.TrySeveralTimesAsync(() => TrySendSubmissionResultsToQueue(submission), 5, () => Task.Delay(TimeSpan.FromMilliseconds(1)));
		}

		private async Task<bool> TrySendSubmissionResultsToQueue(XQueueExerciseSubmission submission)
		{
			var checking = submission.Submission.AutomaticChecking;
			var message = checking.IsCompilationError ? checking.CompilationError.Text : checking.Output.Text;
			return await client.PutResult(new XQueueResult
			{
				header = submission.XQueueHeader,
				Body = new XQueueResultBody
				{
					IsCorrect = checking.IsRightAnswer,
					Message = message,
					Score = checking.IsRightAnswer ? 1 : 0,
				}
			});
		}

		private async Task ProcessOneSubmissionFromQueue()
		{
			var submission = await client.GetSubmission(watcher.QueueName);
			if (submission == null)
			{
				return;
			}

			log.Info($"Found new submission in queue {watcher.QueueName}: {submission.JsonSerialize()}");

			OnSubmission.Invoke(this, new SubmissionEventArgs
			{
				Submission = submission
			});
		}
	}

	public class SubmissionEventArgs : EventArgs
	{
		public XQueueSubmission Submission;
	}
}