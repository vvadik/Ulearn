using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Ionic.Zip;
using log4net;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core.Configuration;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Exercises.Blocks;

namespace Ulearn.Core.Helpers
{
	public static class ExerciseCheckerZipsCache
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ExerciseCheckerZipsCache));

		private static readonly DirectoryInfo cacheDirectory;
		private static readonly UlearnConfiguration configuration;
		private static readonly HashSet<string> coursesLockedForDelete;
		private static readonly ConcurrentDictionary<string, int> courseToFileLocksNumber;

		static ExerciseCheckerZipsCache()
		{
			configuration = ApplicationConfiguration.Read<UlearnConfiguration>();
			cacheDirectory = GetCacheDirectory();
			coursesLockedForDelete = new HashSet<string>();
			courseToFileLocksNumber = new ConcurrentDictionary<string, int>();
			cacheDirectory.EnsureExists();
		}

		private static DirectoryInfo GetCacheDirectory()
		{
			var directory = configuration.ExerciseCheckerZipsDirectory;
			if (!string.IsNullOrEmpty(directory))
				return new DirectoryInfo(directory);

			return CourseManager.GetCoursesDirectory().GetSubdirectory("ExerciseCheckerZips");
		}

		public static byte[] GetZip(IExerciseCheckerZipBuilder zipBuilder, string userCodeFilePath, byte[] userCodeFileContent)
		{
			var courseId = zipBuilder.CourseId;
			var slide = zipBuilder.Slide;
			var courseDirectory = cacheDirectory.GetSubdirectory(courseId);
			var zipFile = courseDirectory.GetFile($"{slide.Id}.zip");
			byte[] zipBytes = null;
			if (!zipFile.Exists || coursesLockedForDelete.Contains(courseId))
			{
				courseDirectory.EnsureExists();
				log.Info($"Собираю zip-архив с упражнением: курс {courseId}, слайд «{slide.Title}» ({slide.Id}), файл {zipFile.FullName}");
				zipBytes = zipBuilder.GetZipBytesForChecker();
				if(!coursesLockedForDelete.Contains(courseId))
					WithLock(courseId, () => SaveFileOnDisk(zipFile, zipBytes));
			}

			MemoryStream zipMemoryStream = null;
			if (zipBytes != null)
				zipMemoryStream = new MemoryStream(zipBytes);
			else
			{
				WithLock(courseId, () =>
				{
					using (var stream = zipFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						zipMemoryStream = new MemoryStream();
						stream.CopyTo(zipMemoryStream);
					}
				});
				zipMemoryStream.Position = 0;
			}

			return AddUserCodeToZip(zipMemoryStream, userCodeFilePath, userCodeFileContent, courseId, slide);
		}

		private static void SaveFileOnDisk(FileInfo zipFile, byte[] zipBytes)
		{
			try
			{
				using (var fileStream = zipFile.Open(FileMode.CreateNew, FileAccess.Write, FileShare.None))
					fileStream.Write(zipBytes, 0, zipBytes.Length);
			}
			catch (Exception ex)
			{
				// ignore
			}
		}

		private static void WithLock(string courseId, Action action)
		{
			courseToFileLocksNumber.AddOrUpdate(courseId, 1, (key, value) => ++value);
			try
			{
				action();
			}
			finally
			{
				while (true)
				{
					var value = courseToFileLocksNumber[courseId];
					if(courseToFileLocksNumber.TryUpdate(courseId, value - 1, value));
					break;
				}
			}
		}

		private static byte[] AddUserCodeToZip(MemoryStream inputStream, string userCodeFilePath, byte[] userCodeFileContent, string courseId, Slide slide)
		{
			var sw = Stopwatch.StartNew();
			var resultStream = new MemoryStream();
			using (var zip = ZipFile.Read(inputStream))
			{
				if(zip.ContainsEntry(userCodeFilePath))
					zip.UpdateEntry(userCodeFilePath, userCodeFileContent);
				else
					zip.AddEntry(userCodeFilePath, userCodeFileContent);
				zip.Save(resultStream);
			}
			var result = resultStream.ToArray();
			log.Info($"Добавил код студента в zip-архив с упражнением: курс {courseId}, слайд «{slide.Title}» ({slide.Id}) elapsed {sw.ElapsedMilliseconds} ms");
			return result;
		}

		public static void DeleteCourseZips(string courseId)
		{
			log.Info($"Очищаю папку со сгенерированными zip-архивами для упражнений из курса {courseId}");

			var courseDirectory = cacheDirectory.GetSubdirectory(courseId);
			courseDirectory.EnsureExists();
			
			coursesLockedForDelete.Add(courseId);
			try
			{
				void ClearDirectory() => FuncUtils.TrySeveralTimes(() => courseDirectory.ClearDirectory(), 3);
				if (!courseToFileLocksNumber.ContainsKey(courseId))
					ClearDirectory();
				else
				{
					while (courseToFileLocksNumber[courseId] > 0)
						Thread.Sleep(10);
					ClearDirectory();
				}
			}
			finally
			{
				coursesLockedForDelete.Remove(courseId);
			}
		}
	}
}