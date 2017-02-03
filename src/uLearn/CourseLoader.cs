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
			if (string.IsNullOrEmpty(settings.Title))
				settings.Title = GetCourseTitleFromFile(dir);

			var units = LoadUnits(dir, settings).ToList();
			CheckDuplicateSlideIds(units.SelectMany(u => u.Slides));

			return new Course(courseId, units, settings, dir);
		}

		private static IEnumerable<Unit> LoadUnits(DirectoryInfo dir, CourseSettings settings)
		{
			var unitsDirectories = dir.GetDirectories().OrderBy(d => d.Name);

			var unitsIds = new HashSet<Guid>();
			var unitsUrls = new HashSet<string>();
			var slideIndex = 0;
			foreach (var unitDirectory in unitsDirectories)
			{
				var unit = UnitLoader.LoadUnit(unitDirectory, settings, slideIndex);

				if (unitsIds.Contains(unit.Id))
					throw new CourseLoadingException($"Ошибка в курсе \"{settings.Title}\". Повторяющийся идентификатор модуля: {unit.Id}. Идентификаторы модулей должны быть уникальными");
				unitsIds.Add(unit.Id);

				if (unitsUrls.Contains(unit.Url))
					throw new CourseLoadingException($"Ошибка в курсе \"{settings.Title}\". Повторяющийся url-адрес модуля: {unit.Url}. Url-адреса модулей должны быть уникальными");
				unitsUrls.Add(unit.Url);

				yield return unit;

				slideIndex += unit.Slides.Count;
			}
		}

		private static string GetCourseTitleFromFile(DirectoryInfo dir)
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
					"Идентификаторы слайдов (SlideId) должны быть уникальными.\n" + 
					"Слайды с повторяющимися идентификаторами:\n" +
					string.Join("\n", badSlides.Select(x => string.Join("\n", x))));
		}
	}

	public class CourseLoadingException : Exception
	{
		public CourseLoadingException(string message) : base(message)
		{
		}

		public CourseLoadingException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}