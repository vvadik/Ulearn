using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Database.Repos;
using Microsoft.Extensions.DependencyInjection;
using Vostok.Logging.Abstractions;
using Ulearn.Core.Courses.Manager;

namespace Database
{
	public class WebCourseManager : CourseManager, IWebCourseManager, ICourseUpdater
	{
		private static ILog log => LogProvider.Get().ForContext(typeof(WebCourseManager));

		private readonly Dictionary<string, Guid> loadedCourseVersions = new Dictionary<string, Guid>();
		private readonly IServiceScopeFactory serviceScopeFactory;

		public WebCourseManager(IServiceScopeFactory serviceScopeFactory)
			: base(GetCoursesDirectory())
		{
			this.serviceScopeFactory = serviceScopeFactory;
		}

		private readonly object @lock = new object();

		public async Task UpdateCourses()
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
				LoadCoursesIfNotYet();
				var coursesRepo = (CoursesRepo)scope.ServiceProvider.GetService(typeof(ICoursesRepo));
				var publishedCourseVersions = await coursesRepo.GetPublishedCourseVersions();

				foreach (var courseVersion in publishedCourseVersions)
				{
					if (CourseIsBroken(courseVersion.CourseId))
						continue;
					try
					{
						ReloadCourseIfLoadedAndPublishedVersionsAreDifferent(courseVersion.CourseId, courseVersion);
					}
					catch (FileNotFoundException)
					{
						/* Sometimes zip-archive with course has been deleted already. It's strange but ok */
						log.Warn("Это странно, что я не смог загрузить с диска курс, который, если верить базе данных, был опубликован. Но ничего, просто проигнорирую");
					}
				}
			}
		}

		public async Task UpdateTempCourses()
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var tempCoursesRepo = (TempCoursesRepo)scope.ServiceProvider.GetService(typeof(ITempCoursesRepo));
				await LoadTempCoursesIfNotYetAsync(tempCoursesRepo);
			}
		}

		private async Task LoadTempCoursesIfNotYetAsync(ITempCoursesRepo tempCoursesRepo)
		{
			var tempCourses = await tempCoursesRepo.GetTempCoursesAsync();
			tempCourses
				.Where(tempCourse => !CourseStorageInstance.HasCourse(tempCourse.CourseId))
				.ToList()
				.ForEach(course => TryReloadCourse(course.CourseId));
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
					var actual = isCourseLoaded ? loadedVersionId.ToString() : "<none>";
					log.Info($"Загруженная версия курса {courseId} отличается от актуальной ({actual} != {publishedVersion.Id}). Обновляю курс.");
					TryReloadCourse(courseId);
				}

				loadedCourseVersions[courseId.ToLower()] = publishedVersion.Id;
			}
		}
	}
}