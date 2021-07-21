using System;
using System.IO;
using System.Threading.Tasks;
using Database.Repos;
using Microsoft.Extensions.DependencyInjection;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Manager;
using Vostok.Logging.Abstractions;

namespace Database
{
	public class SlaveCourseManager : CourseManager, ISlaveCourseManager
	{
		private static ILog log => LogProvider.Get().ForContext(typeof(SlaveCourseManager));
		private readonly IServiceScopeFactory serviceScopeFactory;

		public SlaveCourseManager(IServiceScopeFactory serviceScopeFactory)
			: base(GetCoursesDirectory())
		{
			this.serviceScopeFactory = serviceScopeFactory;
		}

		public virtual async Task UpdateCourses()
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var coursesRepo = scope.ServiceProvider.GetService<ICoursesRepo>();
				var publishedCourseVersions = await coursesRepo.GetPublishedCourseVersions();
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
		}

		public virtual async Task UpdateTempCourses()
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var tempCoursesRepo = scope.ServiceProvider.GetService<ITempCoursesRepo>();
				var tempCourses = await tempCoursesRepo.GetTempCoursesAsync();
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
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var coursesRepo = scope.ServiceProvider.GetService<ICoursesRepo>();
				var hasCourse = await coursesRepo.GetPublishedCourseVersion(courseId) != null;
				if (!hasCourse)
				{
					var helpVersionFile = await coursesRepo.GetPublishedVersionFile(exampleCourseId);
					using (var exampleCourseZip = SaveVersionZipToTemporaryDirectory(courseId, versionId, new MemoryStream(helpVersionFile.File)))
					{
						CreateCourseFromExample(courseId, courseTitle, exampleCourseZip.FileInfo);
						await coursesRepo.AddCourseVersion(courseId, versionId, userId, null, null, null, null, await exampleCourseZip.FileInfo.ReadAllContentAsync());
					}
					await coursesRepo.MarkCourseVersionAsPublished(versionId);
				}
				return !hasCourse;
			}
		}
	}
}