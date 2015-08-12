using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using uLearn.CSharp;
using uLearn.Model;
using uLearn.Quizes;

namespace uLearn
{
	public static class FileSystemExtensions
	{
		public static bool HasSubdirectory(this DirectoryInfo root, string name)
		{
			return root.Subdirectory(name).Exists;
		}

		public static DirectoryInfo Subdirectory(this DirectoryInfo root, string name)
		{
			return new DirectoryInfo(Path.Combine(root.FullName, name));
		}
	}
	public class CourseLoader
	{
		private static readonly IList<ISlideLoader> SlideLoaders = new ISlideLoader[] { new LessonSlideLoader(), new CSharpSlideLoader(), new QuizSlideLoader() };
		
		public Course LoadCourse(DirectoryInfo dir)
		{
			var courseId = dir.Name;

			dir = dir.HasSubdirectory("Slides") ? dir.Subdirectory("Slides") : dir;
			var settings = CourseSettings.Load(dir);
			var slides = LoadSlides(dir, settings).ToArray();
			CheckDuplicateSlideIds(slides);
			var notes = LoadInstructorNotes(dir, courseId);
			var title = settings.Title ?? GetTitle(dir);
			return new Course(courseId, title, slides, notes, settings);
		}

		private static InstructorNote[] LoadInstructorNotes(DirectoryInfo dir, string courseId)
		{
			return dir.GetDirectories()
				.Select(unitDir => new
				{
					unitDir,
					notes = unitDir.GetFile("InstructorNotes.md")
				})
				.Where(unit => unit.notes.Exists)
				.Select(unit => new InstructorNote(unit.notes.ContentAsUtf8(), courseId, GetTitle(unit.unitDir), unit.notes))
				.ToArray();
		}

		private static IEnumerable<Slide> LoadSlides(DirectoryInfo dir, CourseSettings settings)
		{
			var unitDirs = dir
				.GetDirectories()
				.OrderBy(d => d.Name);
			return unitDirs
				.SelectMany(info => LoadUnit(info, settings))
				.Select((makeSlide, index) => makeSlide(index));
		}

		private static IEnumerable<Func<int, Slide>> LoadUnit(DirectoryInfo unitDir, CourseSettings settings)
		{
			var unitTitle = GetTitle(unitDir);
			return unitDir.GetFiles()
				.Where(f => IsSlideFile(f.Name))
				.OrderBy(f => f.Name)
				.Select<FileInfo, Func<int, Slide>>(f => i => LoadSlide(f, unitTitle, i, settings));
		}

		private static bool IsSlideFile(string name)
		{
			//S001_slide.ext
			var id = name.Split(new[] { '_', '-', ' ' }, 2)[0];
			return id.StartsWith("S", StringComparison.InvariantCultureIgnoreCase)
					&& id.Skip(1).All(char.IsDigit);
		}

		private static Slide LoadSlide(FileInfo file, string unitTitle, int slideIndex, CourseSettings settings)
		{
			try
			{
				var slideLoader = SlideLoaders
					.FirstOrDefault(loader => file.FullName.EndsWith(loader.Extension, StringComparison.InvariantCultureIgnoreCase));
				if (slideLoader == null)
					throw new Exception("Unknown slide format " + file);
				return slideLoader.Load(file, unitTitle, slideIndex, settings);
			}
			catch (Exception e)
			{
				throw new Exception("Error loading slide " + file.FullName, e);
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
				throw new Exception(
					"Duplicate SlideId:\n" +
					string.Join("\n", badSlides.Select(x => string.Join("\n", x))));
		}
	}
}