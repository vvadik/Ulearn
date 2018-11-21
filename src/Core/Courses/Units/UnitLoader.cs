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
		
		public Unit LoadUnit(DirectoryInfo unitDir, CourseSettings courseSettings, string courseId, int firstSlideIndex)
		{
			var unitFile = unitDir.GetFile("unit.xml");
			var unitSettings = unitFile.Exists
				? UnitSettings.Load(unitFile, courseSettings)
				: UnitSettings.CreateByTitle(GetUnitTitleFromFile(unitDir), courseSettings);

			var unit = new Unit(unitSettings, unitDir);

			unit.Slides = unitDir.GetFiles()
				.Where(f => IsSlideFile(f.Name))
				.OrderBy(f => f.Name)
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