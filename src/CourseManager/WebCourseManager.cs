using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Database.DataContexts;
using log4net;
using uLearn;

namespace CourseManager
{
    public class WebCourseManager : uLearn.CourseManager
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(WebCourseManager));

		private readonly Dictionary<string, Guid> loadedCourseVersions = new Dictionary<string, Guid>();
		private readonly ConcurrentDictionary<string, DateTime> courseVersionFetchTime = new ConcurrentDictionary<string, DateTime>();
		private readonly TimeSpan fetchCourseVersionEvery = TimeSpan.FromMinutes(1);

		private WebCourseManager()
			: base(GetCoursesDirectory())
		{
		}

		private static DirectoryInfo GetCoursesDirectory()
		{
			var coursesDirectory = ConfigurationManager.AppSettings["ulearn.coursesDirectory"];
			if (string.IsNullOrEmpty(coursesDirectory))
				coursesDirectory = Utils.GetAppPath();

			return new DirectoryInfo(coursesDirectory);
		}

		private readonly object @lock = new object();
		public override Course GetCourse(string courseId)
		{
			var course = base.GetCourse(courseId);
			if (IsCourseVersionWasUpdatedRecent(courseId))
				return course;

			courseVersionFetchTime[courseId] = DateTime.Now;
			var coursesRepo = new CoursesRepo();
			var publishedVersion = coursesRepo.GetPublishedCourseVersion(courseId);

			if (publishedVersion == null)
				return course;

			lock (@lock)
			{
				Guid loadedVersionId;
				if (loadedCourseVersions.TryGetValue(courseId, out loadedVersionId)
					&& loadedVersionId != publishedVersion.Id)
				{
					log.Info($"Загруженная версия курса {courseId} отличается от актуальной. Обновляю курс.");
					course = ReloadCourse(courseId);
				}
				loadedCourseVersions[courseId] = publishedVersion.Id;
			}
			return course;
		}

		private bool IsCourseVersionWasUpdatedRecent(string courseId)
		{
			DateTime lastFetchTime;
			if (courseVersionFetchTime.TryGetValue(courseId, out lastFetchTime))
				return lastFetchTime > DateTime.Now.Subtract(fetchCourseVersionEvery);
			return false;
		}

		public void UpdateCourseVersion(string courseId, Guid versionId)
		{
			lock (@lock)
			{
				loadedCourseVersions[courseId] = versionId;
			}
		}

		public static readonly WebCourseManager Instance = new WebCourseManager();
	}
}
