using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using uLearn.Web.DataContexts;

namespace uLearn.Web
{
	public class WebCourseManager : CourseManager
	{
		private readonly CoursesRepo coursesRepo = new CoursesRepo();
		private readonly Dictionary<string, Guid> loadedCourseVersions = new Dictionary<string, Guid>();
		private readonly Dictionary<string, DateTime> courseVersionFetchTime = new Dictionary<string, DateTime>();
		private readonly TimeSpan fetchCourseVersionEvery = TimeSpan.FromMinutes(1);

		public WebCourseManager() 
			: base(new DirectoryInfo(GetAppPath()))
		{
		}

		private static string GetAppPath()
		{
			return HostingEnvironment.ApplicationPhysicalPath ?? "..";
		}

		private readonly object @lock = new object();
		public override Course GetCourse(string courseId)
		{
			var course = base.GetCourse(courseId);
			if (IsCourseVersionWasUpdatedRecent(courseId))
				return course;
			var publishedVersion = coursesRepo.GetPublishedCourseVersion(courseId);
			if (publishedVersion == null)
				return course;

			courseVersionFetchTime[courseId] = DateTime.Now;
			lock (@lock)
			{
				Guid loadedVersionId;
				if (loadedCourseVersions.TryGetValue(courseId, out loadedVersionId)
					&& loadedVersionId != publishedVersion.Id)
					course = ReloadCourse(courseId);
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