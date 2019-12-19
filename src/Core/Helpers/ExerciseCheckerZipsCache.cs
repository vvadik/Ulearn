using System.Diagnostics;
using System.IO;
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

		static ExerciseCheckerZipsCache()
		{
			configuration = ApplicationConfiguration.Read<UlearnConfiguration>();
			cacheDirectory = GetCacheDirectory();
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
			if (!zipFile.Exists)
			{
				courseDirectory.EnsureExists();
				log.Info($"Собираю zip-архив с упражнением: курс {courseId}, слайд «{slide.Title}» ({slide.Id}), файл {zipFile.FullName}");
				var bytes = zipBuilder.GetZipBytesForChecker();
				using(var fileStream = zipFile.OpenWrite())
					fileStream.Write(bytes, 0, bytes.Length);
			}

			return AddUserCodeToZip(zipFile, userCodeFilePath, userCodeFileContent, courseId, slide);
		}

		private static byte[] AddUserCodeToZip(FileInfo zipFile, string userCodeFilePath, byte[] userCodeFileContent, string courseId, Slide slide)
		{
			var sw = Stopwatch.StartNew();
			var ms = new MemoryStream();
			using (var zip = ZipFile.Read(zipFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
			{
				if(zip.ContainsEntry(userCodeFilePath))
					zip.UpdateEntry(userCodeFilePath, userCodeFileContent);
				else
					zip.AddEntry(userCodeFilePath, userCodeFileContent);
				zip.Save(ms);
			}
			var result = ms.ToArray();
			log.Info($"Добавил код студента в zip-архив с упражнением: курс {courseId}, слайд «{slide.Title}» ({slide.Id}), файл {zipFile.FullName} elapsed {sw.ElapsedMilliseconds} ms");
			return result;
		}

		public static void DeleteCourseZips(string courseId)
		{
			log.Info($"Очищаю папку со сгенерированными zip-архивами для упражнений из курса {courseId}");

			var courseDirectory = cacheDirectory.GetSubdirectory(courseId);
			courseDirectory.EnsureExists();

			FuncUtils.TrySeveralTimes(() => courseDirectory.ClearDirectory(), 3);
		}
	}
}