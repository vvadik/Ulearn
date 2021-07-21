using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Microsoft.Extensions.DependencyInjection;
using Vostok.Logging.Abstractions;

namespace Ulearn.Web.Api.Utils
{
	public class MasterCourseManager : SlaveCourseManager, IMasterCourseManager
	{
		private static ILog log => LogProvider.Get().ForContext(typeof(MasterCourseManager));

		private readonly Dictionary<string, Guid> loadedCourseVersions = new Dictionary<string, Guid>();
		private readonly IServiceScopeFactory serviceScopeFactory;

		public MasterCourseManager(IServiceScopeFactory serviceScopeFactory)
			: base(serviceScopeFactory)
		{
			this.serviceScopeFactory = serviceScopeFactory;
		}

		private readonly object @lock = new object();

		public override async Task UpdateCourses()
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
				LoadCoursesIfNotYet();
				var coursesRepo = scope.ServiceProvider.GetService<ICoursesRepo>();
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

		public override async Task UpdateTempCourses()
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var tempCoursesRepo = scope.ServiceProvider.GetService<TempCoursesRepo>();
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

		protected override async Task LoadCourseZipsToDiskFromExternalStorage(IEnumerable<string> existingOnDiskCourseIds)
		{
			log.Info("Загружаю курсы из БД");
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var coursesRepo = scope.ServiceProvider.GetService<ICoursesRepo>();
				var publishedCourseVersions = await coursesRepo.GetPublishedCourseVersions();
				var coursesNotOnDisk = publishedCourseVersions.Where(c => !existingOnDiskCourseIds.Contains(c.CourseId, StringComparer.OrdinalIgnoreCase));
				foreach (var publishedCourseVersion in coursesNotOnDisk)
				{
					var fileInDb = await coursesRepo.GetVersionFile(publishedCourseVersion.Id);
					try
					{
						var stagingCourseFile = GetStagingCourseFile(fileInDb.CourseId);
						File.WriteAllBytes(stagingCourseFile.FullName, fileInDb.File);
						var versionCourseFile = GetCourseVersionFile(fileInDb.CourseVersionId);
						if (!versionCourseFile.Exists)
							File.WriteAllBytes(versionCourseFile.FullName, fileInDb.File);
						UnzipFile(stagingCourseFile, GetExtractedCourseDirectory(fileInDb.CourseId));
					}
					catch (Exception ex)
					{
						log.Error(ex, $"Не смог загрузить {fileInDb.CourseId} из базы данных");
					}
				}
			}
		}
	}
}