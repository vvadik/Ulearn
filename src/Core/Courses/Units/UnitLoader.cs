using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Slides;

namespace Ulearn.Core.Courses.Units
{
	public class UnitLoader : IUnitLoader
	{
		private readonly ISlideLoader slideLoader;

		public UnitLoader(ISlideLoader slideLoader)
		{
			this.slideLoader = slideLoader;
		}
		
		public Unit Load(FileInfo unitFile, CourseSettings courseSettings, string courseId, int firstSlideIndex)
		{
			var unitDirectory = unitFile.Directory;
			if (unitDirectory == null)
				throw new CourseLoadingException($"Не могу загрузить модуль из {unitFile.FullName}: не могу определить, из какой папки читать файлы со слайдами.");
			
			UnitSettings unitSettings;
			if (unitFile.Exists)
				unitSettings = UnitSettings.Load(unitFile, courseSettings);
			else
			{
				try
				{
					unitSettings = UnitSettings.CreateByTitle(GetUnitTitleFromFile(unitDirectory), courseSettings);
				}
				catch (Exception e)
				{
					throw new CourseLoadingException(
						$"Не удалось прочитать настройки курса. Скорее всего, отсутствует или неправильно заполнен файл модуля {unitFile.Name} ({unitFile.FullName})."
					);
				}
			}

			var unit = new Unit(unitSettings, unitDirectory);

			var slideFiles = unitSettings
				.SlidesPaths
				.SelectMany(path => unitDirectory.GetFiles(path, SearchOption.TopDirectoryOnly).OrderBy(f => f.FullName, StringComparer.InvariantCultureIgnoreCase))
				.Distinct();
				
			unit.Slides = slideFiles
				.Select((f, internalIndex) => LoadSlide(f, unit, firstSlideIndex + internalIndex, courseId, courseSettings))
				.ToList();

			unit.LoadInstructorNote();

			return unit;
		}

		private Slide LoadSlide(FileInfo file, Unit unit, int slideIndex, string courseId, CourseSettings courseSettings)
		{
			try
			{
				return slideLoader.Load(file, slideIndex, unit, courseId, courseSettings);
			}
			catch (Exception e)
			{
				if (e.GetType().IsSubclassOf(typeof(CourseLoadingException)))
					throw;
				throw new CourseLoadingException($"Не могу загрузить слайд из файла {file.FullName}", e);
			}
		}

		[Obsolete("It's old slide filename format. Don't use it now.")]
		public static bool IsSlideFile(string name)
		{
			//S001_slide.ext
			var id = name.Split(new[] { '_', '-', ' ' }, 2)[0];
			return id.StartsWith("S", StringComparison.InvariantCultureIgnoreCase)
					&& id.Skip(1).All(char.IsDigit);
		}

		private static string GetUnitTitleFromFile(DirectoryInfo dir)
		{
			return dir.GetFile("Title.txt").ContentAsUtf8();
		}
	}
}