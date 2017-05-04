using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using uLearn.CSharp;
using uLearn.Extensions;
using uLearn.Model;
using uLearn.Quizes;

namespace uLearn
{
	public class UnitLoader
	{
		private static readonly IList<ISlideLoader> slideLoaders = new ISlideLoader[]
		{
			new LessonSlideLoader(),
			new CSharpSlideLoader(),
			new QuizSlideLoader()
		};

		public static Unit LoadUnit(DirectoryInfo unitDir, CourseSettings courseSettings, int firstSlideIndex)
		{
			var unitFile = unitDir.GetFile("Unit.xml");
			var unitSettings = unitFile.Exists
				? UnitSettings.Load(unitFile, courseSettings)
				: UnitSettings.CreateByTitle(GetUnitTitleFromFile(unitDir), courseSettings);

			var unit = new Unit(unitSettings, unitDir);

			unit.Slides = unitDir.GetFiles()
				.Where(f => IsSlideFile(f.Name))
				.OrderBy(f => f.Name)
				.Select((f, internalIndex) => LoadSlide(f, unit, firstSlideIndex + internalIndex, courseSettings))
				.ToList();

			unit.LoadInstructorNote();

			return unit;
		}

		private static Slide LoadSlide(FileInfo file, Unit unit, int slideIndex, CourseSettings courseSettings)
		{
			try
			{
				var slideLoader = slideLoaders
					.FirstOrDefault(loader => file.FullName.EndsWith(loader.Extension, StringComparison.InvariantCultureIgnoreCase));
				if (slideLoader == null)
					throw new CourseLoadingException($"Неизвестный формат слайда в файле {file.FullName}");
				return slideLoader.Load(file, unit, slideIndex, courseSettings);
			}
			catch (Exception e)
			{
				if (e.GetType().IsSubclassOf(typeof(CourseLoadingException)))
					throw;
				throw new CourseLoadingException($"Не могу загрузить слайд из файла {file.FullName}", e);
			}
		}

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