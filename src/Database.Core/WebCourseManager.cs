using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Database.Models;
using Database.Repos;
using log4net;
using Serilog;
using uLearn;

namespace Database
{
	public class WebCourseManager : CourseManager
	{
		private readonly CoursesRepo coursesRepo;
		private readonly ILogger logger;

		private readonly Dictionary<string, Guid> loadedCourseVersions = new Dictionary<string, Guid>();
		private readonly ConcurrentDictionary<string, DateTime> courseVersionFetchTime = new ConcurrentDictionary<string, DateTime>();
		private DateTime lastCoursesListFetchTime = DateTime.MinValue;
		private readonly TimeSpan fetchCourseVersionEvery = TimeSpan.FromSeconds(10);

		public WebCourseManager(CoursesRepo coursesRepo, ILogger logger)
			: base(GetCoursesDirectory())
		{
			this.coursesRepo = coursesRepo;
			this.logger = logger;
		}

		private readonly object @lock = new object();

		public override Course GetCourse(string courseId)
		{
			var course = base.GetCourse(courseId);
			if (IsCourseVersionWasUpdatedRecent(courseId))
				return course;

			courseVersionFetchTime[courseId] = DateTime.Now;
			
			var publishedVersion = coursesRepo.GetPublishedCourseVersion(courseId);

			if (publishedVersion == null)
				return course;

			ReloadCourseIfLoadedAndPublishedVersionsAreDifferent(courseId, publishedVersion);
			return base.GetCourse(courseId);
		}

		public async Task<IEnumerable<Course>> GetCoursesAsync()
		{
			if (lastCoursesListFetchTime > DateTime.Now.Subtract(fetchCourseVersionEvery))
				return base.GetCourses();
				
			var publishedCourseVersions = await coursesRepo.GetPublishedCourseVersionsAsync().ConfigureAwait(false);
			lastCoursesListFetchTime = DateTime.Now;
			foreach (var courseVersion in publishedCourseVersions)
			{
				try
				{
					ReloadCourseIfLoadedAndPublishedVersionsAreDifferent(courseVersion.CourseId, courseVersion);
				}
				catch (FileNotFoundException)
				{
					/* Sometimes zip-archive with course has been deleted already. It's strange but ok */
					logger.Warning("Это странно, что я не смог загрузить с диска курс, который, если верить базе данных, был опубликован. Но ничего, просто проигнорирую");
				}
			}

			return base.GetCourses();
		}

		private bool IsCourseVersionWasUpdatedRecent(string courseId)
		{
			if (courseVersionFetchTime.TryGetValue(courseId, out var lastFetchTime))
				return lastFetchTime > DateTime.Now.Subtract(fetchCourseVersionEvery);
			return false;
		}

		public void UpdateCourseVersion(string courseId, Guid versionId)
		{
			lock (@lock)
			{
				loadedCourseVersions[courseId.ToLower()] = versionId;
			}
		}
		
		private void ReloadCourseIfLoadedAndPublishedVersionsAreDifferent(string courseId, CourseVersion publishedVersion)
		{
			lock (@lock)
			{
				var isCourseLoaded = loadedCourseVersions.TryGetValue(courseId.ToLower(), out var loadedVersionId);
				if ((isCourseLoaded && loadedVersionId != publishedVersion.Id) || !isCourseLoaded)
				{
					logger.Information($"Загруженная версия курса {courseId} отличается от актуальной ({loadedVersionId.ToString() ?? "<none>"} != {publishedVersion.Id}). Обновляю курс.");
					ReloadCourse(courseId);
				}

				loadedCourseVersions[courseId.ToLower()] = publishedVersion.Id;
			}
		}
	}
}