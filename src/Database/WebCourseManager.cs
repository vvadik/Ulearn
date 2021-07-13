using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Database.DataContexts;
using Database.Models;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;
using Ulearn.Core;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides;

namespace Database
{
	public interface ICourseStorage
	{
		Course GetCourse(string courseId);
		[CanBeNull] Course FindCourse(string courseId);
		IEnumerable<Course> GetCourses();
	}
	
	public interface IWebCourseManager
	{
		void UpdateCourseVersion(string courseId, Guid versionId);
		Course GetVersion(Guid versionId);
		FileInfo GetStagingCourseFile(string courseId);
		FileInfo GetStagingTempCourseFile(string courseId);
		DirectoryInfo GetExtractedCourseDirectory(string courseId);
		DirectoryInfo GetExtractedVersionDirectory(Guid versionId);
		FileInfo GetCourseVersionFile(Guid versionId);
		string GetStagingCoursePath(string courseId);
		string GetPackageName(string courseId);
		string GetPackageName(Guid versionId);
		bool TryCreateCourse(string courseId, string courseTitle, Guid firstVersionId);
		void EnsureVersionIsExtracted(Guid versionId);
		bool HasPackageFor(string courseId);
		void WaitWhileCourseIsLocked(string courseId);
		void MoveCourse(Course course, DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory);
		bool TryReloadCourse(string courseId);
		void ReloadCourseNotSafe(string courseId, bool notifyAboutErrors = true);
		void ExtractTempCourseChanges(string tempCourseId);
		bool TryCreateTempCourse(string courseId, string courseTitle, Guid firstVersionId);
		FileInfo GenerateOrFindStudentZip(string courseId, Slide slide);
	}

	public class WebCourseManager : CourseManager, ICourseStorage, IWebCourseManager
	{
		private static ILog log => LogProvider.Get().ForContext(typeof(WebCourseManager));
		private static readonly WebCourseManager courseManagerInstance = new WebCourseManager();
		public static ICourseStorage CourseStorageInstance => courseManagerInstance;
		public static IWebCourseManager CourseManagerInstance => courseManagerInstance;

		private readonly Dictionary<string, Guid> loadedCourseVersions = new Dictionary<string, Guid>();
		private readonly ConcurrentDictionary<string, DateTime> courseVersionFetchTime = new ConcurrentDictionary<string, DateTime>();
		private readonly TimeSpan fetchCourseVersionEvery = TimeSpan.FromMinutes(1);
		private readonly ConcurrentDictionary<string, DateTime> tempCourseUpdateTime = new ConcurrentDictionary<string, DateTime>();
		private readonly TimeSpan tempCourseUpdateEvery = TimeSpan.FromSeconds(1);
		private long allTempCoursesUpdateTime;
		private readonly TimeSpan allTempCoursesUpdateEvery = TimeSpan.FromMinutes(1);

		private List<TempCourse> tempCoursesCache;
		private readonly TimeSpan tempCoursesCacheUpdateEvery = TimeSpan.FromSeconds(0.5);
		private long tempCoursesCacheUpdateTime;

		private WebCourseManager()
			: base(GetCoursesDirectory())
		{
		}

		private readonly object @lock = new object();

		public override Course GetCourse(string courseId)
		{
			Course course;
			try
			{
				LoadCoursesIfNotYet();
				TryReloadTempCourseIfNecessary(courseId);
				course = base.GetCourse(courseId);
			}
			catch (Exception e) when (e is KeyNotFoundException || e is CourseNotFoundException || e is CourseLoadingException)
			{
				course = null;
			}
			catch (AggregateException e)
			{
				var ie = e.InnerException;
				if (ie is KeyNotFoundException || ie is CourseNotFoundException || ie is CourseLoadingException)
					course = null;
				else
					throw;
			}

			if (IsCourseVersionWasUpdatedRecent(courseId) || CourseIsBroken(courseId) || IsTempCourse(courseId))
				return course ?? throw new KeyNotFoundException($"Key {courseId} not found");

			courseVersionFetchTime[courseId] = DateTime.Now;
			var coursesRepo = new CoursesRepo();
			var publishedVersion = coursesRepo.GetPublishedCourseVersion(courseId);

			if (publishedVersion == null)
				return course ?? throw new KeyNotFoundException($"Key {courseId} not found");

			lock (@lock)
			{
				if (loadedCourseVersions.TryGetValue(courseId.ToLower(), out var loadedVersionId)
					&& loadedVersionId != publishedVersion.Id)
				{
					log.Info($"Загруженная версия курса {courseId} отличается от актуальной ({loadedVersionId.ToString()} != {publishedVersion.Id}). Обновляю курс.");
					if (TryReloadCourse(courseId))
						course = base.GetCourse(courseId);
				}

				loadedCourseVersions[courseId.ToLower()] = publishedVersion.Id;
			}

			return course ?? throw new KeyNotFoundException($"Key {courseId} not found");
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

		protected override void LoadCourseZipsToDiskFromExternalStorage(IEnumerable<string> existingOnDiskCourseIds)
		{
			log.Info("Загружаю курсы из БД");
			var coursesRepo = new CoursesRepo();
			var coursesWithCourseFiles = coursesRepo.GetCourseIdsFromCourseFiles().Where(c => !existingOnDiskCourseIds.Contains(c));
			foreach (var courseId in coursesWithCourseFiles)
			{
				var fileInDb = coursesRepo.GetCourseFile(courseId);
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

		public override IEnumerable<Course> GetCourses()
		{
			try
			{
				LoadCoursesIfNotYet();
				TryCheckTempCoursesAndReloadIfNecessary();
			}
			catch (Exception e)
			{
				log.Error(e, "GetCourses exception");
			}
			return base.GetCourses();
		}

		private void TryReloadTempCourseIfNecessary([NotNull]string courseId)
		{
			try
			{
				if (IsTempCourseUpdatedRecent(courseId))
					return;

				var tempCourses = GetTempCoursesWithCache();
				var tempCourse = tempCourses.FirstOrDefault(tc => string.Equals(tc.CourseId, courseId, StringComparison.OrdinalIgnoreCase));
				if (tempCourse == null)
					return;
				Course course = null;
				try
				{
					course = base.GetCourse(courseId); // Не используется FindCourse, иначе бесконечная рекурсия
				}
				catch (Exception ex)
				{
					log.Error(ex);
				}

				if (tempCourse.LastUpdateTime < tempCourse.LoadingTime)
					RemoveCourseFromBroken(courseId);
				if (CourseIsBroken(courseId))
				{
					tempCourseUpdateTime[courseId] = DateTime.Now;
					return;
				}
				if (course == null || tempCourse.LastUpdateTime < tempCourse.LoadingTime)
				{
					TryReloadCourse(courseId);
					var tempCoursesRepo = new TempCoursesRepo();
					tempCoursesRepo.UpdateTempCourseLastUpdateTime(courseId);
					tempCourseUpdateTime[courseId] = DateTime.Now;
				} else if (tempCourse.LastUpdateTime > tempCourse.LoadingTime)
					tempCourseUpdateTime[courseId] = DateTime.Now;
			}
			catch (Exception ex)
			{
				log.Error(ex);
			}
		}
		
		private void TryCheckTempCoursesAndReloadIfNecessary()
		{
			try
			{
				if (new DateTime(allTempCoursesUpdateTime) > DateTime.Now.Subtract(allTempCoursesUpdateEvery))
					return;
				Interlocked.Exchange(ref allTempCoursesUpdateTime, DateTime.Now.Ticks);

				var tempCourses = GetTempCoursesWithCache();
				foreach (var tempCourse in tempCourses)
				{
					var courseId = tempCourse.CourseId;
					if (CourseIsBroken(courseId))
						continue;
					Course course = null;
					try
					{
						course = base.GetCourse(courseId); // Не используется FindCourse, иначе бесконечная рекурсия
					}
					catch (Exception ex)
					{
						log.Error(ex);
					}
					if (course == null || course.GetSlidesNotSafe().Count == 0)
					{
						TryReloadCourse(courseId);
						var tempCoursesRepo = new TempCoursesRepo();
						tempCoursesRepo.UpdateTempCourseLastUpdateTime(courseId);
						tempCourseUpdateTime[courseId] = DateTime.Now;
					} else if (tempCourse.LastUpdateTime > tempCourse.LoadingTime)
						tempCourseUpdateTime[courseId] = DateTime.Now;
				}
			}
			catch (Exception ex)
			{
				log.Error(ex);
			}
		}

		private List<TempCourse> GetTempCoursesWithCache()
		{
			if (tempCoursesCache != null && new DateTime(tempCoursesCacheUpdateTime) > DateTime.Now.Subtract(tempCoursesCacheUpdateEvery))
				return tempCoursesCache;
			Interlocked.Exchange(ref tempCoursesCacheUpdateTime, DateTime.Now.Ticks);

			var tempCoursesRepo = new TempCoursesRepo();
			tempCoursesCache = tempCoursesRepo.GetTempCoursesNoTracking();
			return tempCoursesCache;
		}

		private bool IsTempCourseUpdatedRecent(string courseId)
		{
			if (tempCourseUpdateTime.TryGetValue(courseId, out var lastFetchTime))
				return lastFetchTime > DateTime.Now.Subtract(tempCourseUpdateEvery);
			return false;
		}

		public bool IsTempCourse(string courseId)
		{
			return GetTempCoursesWithCache().Any(c => string.Equals(c.CourseId, courseId, StringComparison.OrdinalIgnoreCase));
		}
	}
}