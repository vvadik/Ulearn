using System.IO;
using Vostok.Logging.Abstractions;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core.Configuration;
using Ulearn.Core.Courses.Slides;

namespace Ulearn.Core.Helpers
{
	public class ExerciseStudentZipsCache
	{
		private static ILog log => LogProvider.Get().ForContext(typeof(ExerciseStudentZipsCache));

		private readonly DirectoryInfo cacheDirectory;
		private readonly ExerciseStudentZipBuilder builder;
		private static readonly UlearnConfiguration configuration;

		static ExerciseStudentZipsCache()
		{
			configuration = ApplicationConfiguration.Read<UlearnConfiguration>();
		}

		public ExerciseStudentZipsCache()
		{
			cacheDirectory = GetCacheDirectory();
			cacheDirectory.EnsureExists();
			builder = new ExerciseStudentZipBuilder();
		}

		private static DirectoryInfo GetCacheDirectory()
		{
			var directory = configuration.ExerciseStudentZipsDirectory;
			if (!string.IsNullOrEmpty(directory))
				return new DirectoryInfo(directory);

			return CourseManager.GetCoursesDirectory().GetSubdirectory("ExerciseStudentZips");
		}

		public FileInfo GenerateOrFindZip(string courseId, Slide slide)
		{
			var courseDirectory = cacheDirectory.GetSubdirectory(courseId);
			var zipFile = courseDirectory.GetFile($"{slide.Id}.zip");
			if (!zipFile.Exists)
			{
				courseDirectory.EnsureExists();
				log.Info($"Собираю zip-архив с упражнением: курс {courseId}, слайд «{slide.Title}» ({slide.Id}), файл {zipFile.FullName}");
				builder.BuildStudentZip(slide, zipFile);
			}

			return zipFile;
		}

		public void DeleteCourseZips(string courseId)
		{
			log.Info($"Очищаю папку со сгенерированными zip-архивами для упражнений из курса {courseId}");

			var courseDirectory = cacheDirectory.GetSubdirectory(courseId);
			courseDirectory.EnsureExists();

			FuncUtils.TrySeveralTimes(() => courseDirectory.ClearDirectory(), 3);
		}
	}
}