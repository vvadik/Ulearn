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

		private readonly string queueName;
		private readonly XQueueClient client;

		public EventHandler<SubmissionEventArgs> OnSubmission;

		public Watcher(string baseUrl, string queueName, string username, string password)
		{
			client = new XQueueClient(baseUrl, username, password);
			this.queueName = queueName;
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
			var submission = await client.GetSubmission(queueName);
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
