using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace uLearn
{
	public class CourseLoader
	{
		
		public Course LoadCourse(DirectoryInfo dir)
		{
			var courseId = dir.Name;

			dir = dir.HasSubdirectory("Slides") ? dir.Subdirectory("Slides") : dir;
			var settings = CourseSettings.Load(dir);
			var units = LoadUnits(dir, settings).ToList();
			CheckDuplicateSlideIds(units.SelectMany(u => u.Slides));
			var title = settings.Title ?? GetTitle(dir);
			return new Course(courseId, title, units, settings, dir);
		}

		private static IEnumerable<Unit> LoadUnits(DirectoryInfo dir, CourseSettings settings)
		{
			var unitDirs = dir
				.GetDirectories()
				.OrderBy(d => d.Name);
			var slideIndex = 0;
			foreach (var unitDir in unitDirs)
			{
				var unit = UnitLoader.Load(unitDir, settings, slideIndex);
				yield return unit;
				slideIndex += unit.Slides.Count;
			}
		}

		private static string GetTitle(DirectoryInfo dir)
		{
			return dir.GetFile("Title.txt").ContentAsUtf8();
		}

		private static void CheckDuplicateSlideIds(IEnumerable<Slide> slides)
		{
			var badSlides =
				slides.GroupBy(x => x.Id)
					.Where(x => x.Count() != 1)
					.Select(x => x.Select(y => y.Title))
					.ToList();
			if (badSlides.Any())
				throw new CourseLoadingException(
					"»дентификаторы слайдов (SlideId) должны быть уникальными.\n" + 
					"—лайды с повтор€ющимис€ идентификаторами:\n" +
					string.Join("\n", badSlides.Select(x => string.Join("\n", x))));
		}
	}

	public class CourseLoadingException : Exception
	{
		public CourseLoadingException(string message) : base(message)
		{
		}

		public CourseLoadingException(string message, Exception e)
			: base(message, e)
		{
		}
	}
}