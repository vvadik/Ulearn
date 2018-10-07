using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Database.Models;
using Database.Repos;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using uLearn;

namespace Database
{
	public class WebCourseManager : CourseManager
	{
		private readonly ILogger logger;
		private readonly IServiceProvider serviceProvider;

		private readonly Dictionary<string, Guid> loadedCourseVersions = new Dictionary<string, Guid>();
		private readonly ConcurrentDictionary<string, DateTime> courseVersionFetchTime = new ConcurrentDictionary<string, DateTime>();
		private DateTime lastCoursesListFetchTime = DateTime.MinValue;
		private readonly TimeSpan fetchCourseVersionEvery = TimeSpan.FromSeconds(10);

		public WebCourseManager(ILogger logger, IServiceProvider serviceProvider)
			: base(GetCoursesDirectory())
		{
			this.logger = logger;
			this.serviceProvider = serviceProvider;
		}

		private readonly object @lock = new object();

		public override Course GetCourse(string courseId)
		{
			var course = base.GetCourse(courseId);
			if (IsCourseVersionWasUpdatedRecent(courseId))
				return course;

			courseVersionFetchTime[courseId] = DateTime.Now;

			CourseVersion publishedVersion;
			using (serviceProvider.CreateScope())
			{
				/* WebCourseManager is registered as Singleton in ASP.NET DI container, but CoursesRepo should be created for each request */
				var coursesRepo = serviceProvider.GetService<CoursesRepo>();
				publishedVersion = coursesRepo.GetPublishedCourseVersion(courseId);
			}

			if (publishedVersion == null)
				return course;

			ReloadCourseIfLoadedAndPublishedVersionsAreDifferent(courseId, publishedVersion);
			return base.GetCourse(courseId);
		}

		public async Task<IEnumerable<Course>> GetCoursesAsync()
		{
			if (lastCoursesListFetchTime > DateTime.Now.Subtract(fetchCourseVersionEvery))
				return base.GetCourses();

			List<CourseVersion> publishedCourseVersions;
			using (serviceProvider.CreateScope())
			{
				/* WebCourseManager is registered as Singleton in ASP.NET DI container, but CoursesRepo should be created for each request */
				var coursesRepo = serviceProvider.GetService<CoursesRepo>();
				publishedCourseVersions = await coursesRepo.GetPublishedCourseVersionsAsync().ConfigureAwait(false);
			}

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