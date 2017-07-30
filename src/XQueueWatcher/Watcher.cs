using System;
using System.Threading.Tasks;
using log4net;
using XQueue;
using XQueue.Models;

namespace XQueueWatcher
{
	public class Watcher
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Watcher));

		public readonly Database.Models.XQueueWatcher watcher;
		private readonly XQueueClient client;

		public EventHandler<SubmissionEventArgs> OnSubmission;

		public Watcher(Database.Models.XQueueWatcher watcher)
		{
			client = new XQueueClient(watcher.BaseUrl, watcher.UserName, watcher.Password);
			this.watcher = watcher;
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
					log.Error($"Exception occured while make one step in loop: {e.Message}", e);
				}
				await Task.Delay(TimeSpan.FromSeconds(1));
			}
		}

		private async Task OneStep()
		{
			var submission = await client.GetSubmission(watcher.QueueName);
			if (submission == null)
			{
				return;
			}

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