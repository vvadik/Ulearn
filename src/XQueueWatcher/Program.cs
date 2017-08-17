using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Database.DataContexts;
using log4net;
using log4net.Config;
using uLearn;
using uLearn.Extensions;
using XQueue;
using XQueue.Models;

namespace XQueueWatcher
{
	public class Program
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Program));
		private static readonly CourseManager courseManager = WebCourseManager.Instance;

		static void Main(string[] args)
		{
			new Program().StartXQueueWatchers(new CancellationToken());
		}

		public void StartXQueueWatchers(CancellationToken cancellationToken)
		{
			XmlConfigurator.Configure();

			while (true)
			{
				var xQueueRepo = GetNewXQueueRepo();
				var dbWatchers = xQueueRepo.GetXQueueWatchers();

				var tasks = dbWatchers.Select(SafeGetAndProcessSubmissionFromXQueue);
				
				Task.WaitAll(tasks.ToArray(), cancellationToken);
				if (cancellationToken.IsCancellationRequested)
					break;
			}
		}

		/* We need to create new database connection each time for disabling EF's caching */
		private static XQueueRepo GetNewXQueueRepo()
		{
			var db = new ULearnDb();
			return new XQueueRepo(db, courseManager);
		}

		private async Task SafeGetAndProcessSubmissionFromXQueue(Database.Models.XQueueWatcher watcher)
		{
			try
			{
				await GetAndProcessSubmissionFromXQueue(watcher);
			}
			catch (Exception e)
			{
				log.Error(e);
			}
		}

		private async Task GetAndProcessSubmissionFromXQueue(Database.Models.XQueueWatcher watcher)
		{
			var client = new XQueueClient(watcher.BaseUrl, watcher.UserName, watcher.Password);
			
			if (!await client.Login())
			{
				log.Error($"Can\'t login to xqueue {watcher.QueueName} ({watcher.BaseUrl}, user {watcher.UserName})");
				return;
			}

			var submission = await client.GetSubmission(watcher.QueueName);
			if (submission == null)
				return;

			log.Info($"Got new submission in xqueue {watcher.QueueName}: {submission.JsonSerialize()}");

			try
			{
				await ProcessSubmissionFromXQueue(watcher, submission);
			}
			catch (Exception e)
			{
				log.Error($"Error occurred while processing submission from xqueue: {e.Message}", e);
				await client.PutResult(new XQueueResult
				{
					header = submission.header,
					Body = new XQueueResultBody
					{
						Score = 0,
						IsCorrect = false,
						Message = "Sorry, we can't check your code now. Please retry or contact us at support@ulearn.me",
					}
				});
			}
		}

		private async Task ProcessSubmissionFromXQueue(Database.Models.XQueueWatcher watcher, XQueueSubmission submission)
		{
			var xQueueRepo = GetNewXQueueRepo();
			GraderPayload graderPayload;
			try
			{
				graderPayload = submission.Body.GraderPayload.DeserializeJson<GraderPayload>();
			}
			catch (Exception e)
			{
				log.Error($"Can't parse grader payload: {e.Message}");
				log.Info($"Payload is: {submission.Body.GraderPayload}");
				return;
			}

			log.Info($"Add new xqueue submission for course {graderPayload.CourseId}, slide {graderPayload.SlideId}");
			await xQueueRepo.AddXQueueSubmission(
				watcher,
				submission.Header.JsonSerialize(),
				graderPayload.CourseId,
				graderPayload.SlideId,
				submission.Body.StudentResponse
			);
		}
	}

	[DataContract]
	public class GraderPayload
	{
		[DataMember(Name = "course_id")]
		public string CourseId;

		[DataMember(Name = "slide_id")]
		public Guid SlideId;
	}
}