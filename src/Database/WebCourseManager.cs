using System;
using System.IO;
using System.Threading.Tasks;
using Database.DataContexts;
using Ulearn.Common.Extensions;
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

		private WebCourseManager()
			: base(GetCoursesDirectory())
		{
		}

		public async Task UpdateCourses()
		{
			var coursesRepo = new CoursesRepo();
			var publishedCourseVersions = coursesRepo.GetPublishedCourseVersions();
			foreach (var publishedVersion in publishedCourseVersions)
			{
				var courseId = publishedVersion.CourseId;
				var courseInMemory = CourseStorageInstance.FindCourse(publishedVersion.CourseId);
				if (courseInMemory != null && courseInMemory.CourseMeta.Version != publishedVersion.Id)
					continue;
				try
				{
					UpdateCourse(courseId, courseDirectory => CourseLoader.LoadMeta(courseDirectory)!.Version == publishedVersion.Id);
				}
				catch (Exception ex)
				{
					log.Error(ex, $"Не смог обновить курс {courseId}");
				}
			}
		}

		public async Task UpdateTempCourses()
		{
			var tempCoursesRepo = new TempCoursesRepo();
			var tempCourses = tempCoursesRepo.GetTempCoursesNoTracking();
			foreach (var tempCourse in tempCourses)
			{
				var courseId = tempCourse.CourseId;
				var courseInMemory = CourseStorageInstance.FindCourse(courseId);
				if (courseInMemory != null && courseInMemory.CourseMeta.LoadingTime != tempCourse.LoadingTime)
					continue;
				try
				{
					UpdateCourse(courseId, courseDirectory => CourseLoader.LoadMeta(courseDirectory)!.LoadingTime == tempCourse.LoadingTime);
				}
				catch (Exception ex)
				{
					log.Warn(ex, $"Не смог обновить временный курс {courseId}");
				}
			}
		}

		private void UpdateCourse(string courseId, Func<DirectoryInfo, bool> isVersionOnDiskEqualPublished)
		{
			var courseDirectory = GetExtractedCourseDirectory(courseId);
			if (!courseDirectory.Exists || !isVersionOnDiskEqualPublished(courseDirectory))
				return;
			LockCourse(courseId);
			try
			{
				if (!courseDirectory.Exists || !isVersionOnDiskEqualPublished(courseDirectory))
					return;
				var course = loader.Load(courseDirectory);
				CourseStorageUpdaterInstance.AddOrUpdateCourse(course);
			}
			finally
			{
				ReleaseCourse(courseId);
			}
		}

		private const string exampleCourseId = "Help";
		public async Task<bool> CreateCourseIfNotExists(string courseId, Guid versionId, string courseTitle, string userId)
		{
			var coursesRepo = new CoursesRepo();
			var hasCourse = coursesRepo.GetPublishedCourseVersion(courseId) != null;
			if (!hasCourse)
			{
				var helpVersionFile = coursesRepo.GetPublishedVersionFile(exampleCourseId);
				using (var exampleCourseZip = SaveVersionZipToTemporaryDirectory(courseId, versionId, new MemoryStream(helpVersionFile.File)))
				{
					CreateCourseFromExample(courseId, courseTitle, exampleCourseZip.FileInfo);
					await coursesRepo.AddCourseVersion(courseId, versionId, userId, null, null, null, null, await exampleCourseZip.FileInfo.ReadAllContentAsync()
						.ConfigureAwait(false)).ConfigureAwait(false);
				}
				await coursesRepo.MarkCourseVersionAsPublished(versionId).ConfigureAwait(false);
			}
			return !hasCourse;
		}
	}
}