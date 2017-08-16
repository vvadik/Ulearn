using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Database.DataContexts;
using log4net;
using log4net.Config;
using uLearn;
using uLearn.Extensions;

namespace XQueueWatcher
{
	public class Program
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Program));
		private static readonly CourseManager courseManager = WebCourseManager.Instance;
		public CancellationTokenSource CancellationTokenSource { get; private set; }

		static void Main(string[] args)
		{
			new Program().StartXQueueWatchers();
		}

		public void StartXQueueWatchers()
		{
			XmlConfigurator.Configure();

			var db = new ULearnDb();
			var xQueueRepo = new XQueueRepo(db, courseManager);
			var dbWatchers = xQueueRepo.GetXQueueWatchers();

			var tasks = new List<Task>();
			foreach (var dbWatcher in dbWatchers)
			{
				log.Info($"Starting watcher '{dbWatcher.Name}' ({dbWatcher.BaseUrl}, queue {dbWatcher.QueueName})");
				var watcher = new Watcher(dbWatcher, courseManager);
				watcher.OnSubmission += OnSubmission;
				tasks.Add(watcher.Loop());
			}

			CancellationTokenSource = new CancellationTokenSource();
			Task.WaitAll(tasks.ToArray(), CancellationTokenSource.Token);
		}

		private static void OnSubmission(object sender, SubmissionEventArgs args)
		{
			var watcher = sender as Watcher;
			if (watcher == null)
				return;

			var db = new ULearnDb();
			var xQueueRepo = new XQueueRepo(db, courseManager);
			var submission = args.Submission;
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
			xQueueRepo.AddXQueueSubmission(
				watcher.watcher,
				submission.Header.JsonSerialize(),
				graderPayload.CourseId,
				graderPayload.SlideId,
				submission.Body.StudentResponse
			).Wait();
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