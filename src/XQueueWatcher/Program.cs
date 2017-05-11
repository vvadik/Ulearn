using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CourseManager;
using Database.DataContexts;
using log4net;
using uLearn;
using uLearn.Extensions;
using XQueue;

namespace XQueueWatcher
{
	class Program
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Program));

		static void Main(string[] args)
		{
			var db = new ULearnDb();
			var xQueueRepo = new XQueueRepo(db, WebCourseManager.Instance);
			var dbWatchers = xQueueRepo.GetXQueueWatchers();

			var tasks = new List<Task>();
			foreach (var dbWatcher in dbWatchers)
			{
				var watcher = new Watcher(dbWatcher);
				watcher.OnSubmission += OnSubmission;
				tasks.Add(watcher.Loop());
			}
			Task.WaitAll(tasks.ToArray());
		}

		private static void OnSubmission(object sender, SubmissionEventArgs args)
		{
			var watcher = sender as Watcher;
			if (watcher == null)
				return;

			var db = new ULearnDb();
			var xQueueRepo = new XQueueRepo(db, WebCourseManager.Instance);
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
