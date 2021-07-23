using System;
using System.IO;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Microsoft.Extensions.DependencyInjection;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Manager;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Helpers;
using Vostok.Logging.Abstractions;

namespace Ulearn.Web.Api.Utils
{
	public class MasterCourseManager : SlaveCourseManager, IMasterCourseManager
	{
		private static ILog log => LogProvider.Get().ForContext(typeof(MasterCourseManager));

		private readonly IServiceScopeFactory serviceScopeFactory;
		private readonly ExerciseStudentZipsCache exerciseStudentZipsCache;
		private bool tempCourseLoaded;

		public MasterCourseManager(IServiceScopeFactory serviceScopeFactory, ExerciseStudentZipsCache exerciseStudentZipsCache)
			: base(serviceScopeFactory)
		{
			this.serviceScopeFactory = serviceScopeFactory;
			this.exerciseStudentZipsCache = exerciseStudentZipsCache;

			CourseStorageInstance.CourseChangedEvent += courseId =>
			{
				exerciseStudentZipsCache.DeleteCourseZips(courseId);
				ExerciseCheckerZipsCache.DeleteCourseZips(courseId);
			};
		}

		// Невременные курсы не выкладываются на диск сразу, а публикуются в базу и UpdateCourses их обновляет на диске.
		public override async Task UpdateCourses()
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var coursesRepo = scope.ServiceProvider.GetService<ICoursesRepo>();
				var publishedCourseVersions = await coursesRepo.GetPublishedCourseVersions();
				foreach (var publishedCourseVersion in publishedCourseVersions)
				{
					await UpdateCourseToVersionInDirectory(publishedCourseVersion, coursesRepo);
				}
			}
		}

		private async Task UpdateCourseToVersionInDirectory(CourseVersion publishedCourseVersions, ICoursesRepo coursesRepo)
		{
			var courseId = publishedCourseVersions.CourseId;
			var publishedVersionToken = new CourseVersionToken(publishedCourseVersions.Id);
			if (brokenVersions.ContainsKey(publishedVersionToken))
				return;
			var courseInMemory = CourseStorageInstance.FindCourse(courseId);
			if (courseInMemory != null && courseInMemory.CourseVersionToken == publishedVersionToken)
				return;
			var courseFile = await coursesRepo.GetVersionFile(publishedCourseVersions.Id);
			using (var courseDirectory = await ExtractCourseVersionToTemporaryDirectory(courseId, publishedCourseVersions.Id, courseFile.File))
			{
				var (course, error) = LoadCourseFromDirectory(courseId, publishedCourseVersions.Id, courseDirectory.DirectoryInfo);
				if (error != null)
				{
					brokenVersions.TryAdd(publishedVersionToken, true);
					var message = $"Не смог загрузить с диска в память курс {courseId} версии {publishedVersionToken}";
					log.Error(error, message);
					return;
				}
				using (await CourseLock.Lock(courseId))
				{
					try
					{
						MoveCourse(course, courseDirectory.DirectoryInfo, GetExtractedCourseDirectory(courseId));
						CourseStorageUpdaterInstance.AddOrUpdateCourse(course);
					}
					catch (Exception ex)
					{
						log.Error(ex, $"Не смог переместить курс {courseId} версия {publishedVersionToken} из временной папки в общую");
					}
				}
			}
		}

		// Временные курсы выкладываются на диск сразу контроллером, который их создает, здесь только загружаю курсы с диска при старте
		public override async Task UpdateTempCourses()
		{
			if (!tempCourseLoaded)
			{
				tempCourseLoaded = true;
				await base.UpdateTempCourses();
			}
		}

		public FileInfo GenerateOrFindStudentZip(string courseId, Slide slide)
		{
			return exerciseStudentZipsCache.GenerateOrFindZip(courseId, slide, GetExtractedCourseDirectory(courseId).FullName);
		}
	}
}