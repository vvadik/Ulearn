using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Database.DataContexts;
using Database.Models;
using Vostok.Logging.Abstractions;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Manager;

namespace Database
{
	public class WebCourseManager : CourseManager, IWebCourseManager, ICourseUpdater
	{
		private static ILog log => LogProvider.Get().ForContext(typeof(WebCourseManager));

		private static readonly WebCourseManager courseManagerInstance = new WebCourseManager();
		public static IWebCourseManager CourseManagerInstance => courseManagerInstance;
		public static ICourseUpdater CourseUpdaterInstance => courseManagerInstance;

		private readonly Dictionary<string, Guid> loadedCourseVersions = new Dictionary<string, Guid>();

		private WebCourseManager()
			: base(GetCoursesDirectory())
		{
		}

		private readonly object @lock = new object();

		public async Task UpdateCourses()
		{
			LoadCoursesIfNotYet();
			var coursesRepo = new CoursesRepo();
			var publishedCourseVersions = coursesRepo.GetPublishedCourseVersions();
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

		public async Task UpdateTempCourses()
		{
			TryCheckTempCoursesAndReloadIfNecessary();
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

		public void UpdateCourseVersion(string courseId, Guid versionId)
		{
			lock (@lock)
			{
				loadedCourseVersions[courseId.ToLower()] = versionId;
			}
		}

		private void TryCheckTempCoursesAndReloadIfNecessary()
		{
			try
			{
				var tempCoursesRepo = new TempCoursesRepo();
				var tempCourses = tempCoursesRepo.GetTempCoursesNoTracking();
				foreach (var tempCourse in tempCourses)
				{
					var courseId = tempCourse.CourseId;
					if (CourseIsBroken(courseId))
						continue;
					Course course = null;
					try
					{
						course = CourseStorageInstance.GetCourse(courseId);
					}
					catch (Exception ex)
					{
						log.Error(ex);
					}
					if (course == null || course.GetSlidesNotSafe().Count == 0)
					{
						TryReloadCourse(courseId);
						tempCoursesRepo.UpdateTempCourseLastUpdateTime(courseId);
					}
				}
			}
			catch (Exception ex)
			{
				log.Error(ex);
			}
		}
	}
}