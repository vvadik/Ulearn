using System;
using System.Activities.Debugger;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ionic.Zip;
using uLearn.CSharp;
using uLearn.Quizes;

namespace uLearn
{
	public class CourseManager
	{
		private readonly DirectoryInfo stagedDirectory;
		private readonly DirectoryInfo coursesDirectory;
		private readonly Dictionary<string, Course> courses = new Dictionary<string, Course>(StringComparer.InvariantCultureIgnoreCase);
		private DateTime lastReloadTime = DateTime.MinValue;

		private static readonly IList<ISlideLoader> SlideLoaders = new ISlideLoader[]{new CSharpSlideLoader(), new QuizSlideLoader()};

		public CourseManager(DirectoryInfo baseDirectory)
			: this(
			baseDirectory.GetSubdir("Courses.Staging"),
			baseDirectory.GetSubdir("Courses"))
		{
		}

		public CourseManager(DirectoryInfo stagedDirectory, DirectoryInfo coursesDirectory)
		{
			this.stagedDirectory = stagedDirectory;
			this.coursesDirectory = coursesDirectory;
		}

		public IEnumerable<Course> GetCourses()
		{
			ReloadNewStagedCourses();
			return courses.Values;
		}

		public IEnumerable<StagingPackage> GetStagingPackages()
		{
			return stagedDirectory.GetFiles("*.zip").Select(f => new StagingPackage(f.Name, f.LastWriteTime));
		}

		public string GetStagingPackagePath(string name)
		{
			if (Path.GetInvalidFileNameChars().Any(name.Contains)) 
				throw new Exception(name);
			return stagedDirectory.GetFile(name).FullName;
		}

		public ExerciseSlide GetExerciseSlide(string courseId, int slideIndex)
		{
			var course = GetCourse(courseId);
			return (ExerciseSlide)course.Slides[slideIndex];
		}
		
		public Course GetCourse(string courseId)
		{
			ReloadNewStagedCourses();
			return courses.Get(courseId);
		}

		private static readonly object ReloadLock = new object();
		
		private void ReloadNewStagedCourses()
		{
			lock (ReloadLock)
			{
				var reloadTime = DateTime.Now;
				var courseZips = stagedDirectory.GetFiles("*.zip").Where(d => d.LastWriteTime > lastReloadTime);
				foreach (var zipFile in courseZips)
					try
					{
						ReloadCourseFromZip(zipFile);
					}
					catch (Exception e)
					{
						throw new Exception("Error loading course from " + zipFile.Name, e);
					}
				lastReloadTime = reloadTime;
			}
		}

		private void ReloadCourseFromZip(FileInfo zipFile)
		{
			using (var zip = ZipFile.Read(zipFile.FullName))
			{
				var courseId = Path.GetFileNameWithoutExtension(zipFile.Name);
				var courseDir = coursesDirectory.CreateSubdirectory(courseId);
				Directory.Delete(courseDir.FullName, true);
				courseDir.Create();
				zip.ExtractAll(courseDir.FullName, ExtractExistingFileAction.OverwriteSilently);
				ReloadCourse(courseDir);
			}
		}

		private void ReloadCourse(DirectoryInfo dir)
		{
			var course = LoadCourse(dir);
			courses[course.Id] = course;
		}

		public static Course LoadCourse(DirectoryInfo dir)
		{
			var slides = LoadSlides(dir).ToArray();
			CheckDuplicateSlideIds(slides);

			var courseId = dir.Name;
			var notes = LoadInstructorNotes(dir, courseId);
			var title = GetTitle(dir);
			return new Course(courseId, title, slides, notes);
		}

		private static InstructorNote[] LoadInstructorNotes(DirectoryInfo dir, string courseId)
		{
			return dir.GetDirectories()
				.Select(unitDir => new
				{
					unitDir,
					notes=unitDir.GetFile("InstructorNotes.md")
				})
				.Where(unit => unit.notes.Exists)
				.Select(unit => new InstructorNote(unit.notes.ContentAsUtf8(), courseId, GetTitle(unit.unitDir), unit.notes))
				.ToArray();
		}

		private static IEnumerable<Slide> LoadSlides(DirectoryInfo dir)
		{
			var unitDirs = dir
				.GetDirectories()
				.OrderBy(d => d.Name);
			return unitDirs
				.SelectMany(LoadUnit)
				.Select((makeSlide, index) => makeSlide(index));
		}

		private static IEnumerable<Func<int, Slide>> LoadUnit(DirectoryInfo unitDir)
		{
			var unitTitle = GetTitle(unitDir);
			return unitDir.GetFiles()
				.Where(f => IsSlideFile(f.Name))
				.OrderBy(f => f.Name)
				.Select<FileInfo, Func<int, Slide>>(f => i => LoadSlide(f, unitTitle, i));
		}

		private static bool IsSlideFile(string name)
		{
			//S001_slide.ext
			var id = name.Split(new []{'_', '-', ' '}, 2)[0];
			return id.StartsWith("S", StringComparison.InvariantCultureIgnoreCase)
					&& id.Skip(1).All(char.IsDigit);
		}

		private static Slide LoadSlide(FileInfo file, string unitTitle, int slideIndex)
		{
			try
			{
				var ext = file.Extension.Substring(1);
				var slideLoader = SlideLoaders
					.FirstOrDefault(loader => loader.Extension.Equals(ext, StringComparison.InvariantCultureIgnoreCase));
				if (slideLoader == null)
					throw new Exception("Unknown slide format " + file);
				return slideLoader.Load(file, unitTitle, slideIndex);
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

	public class StagingPackage
	{
		public StagingPackage(string name, DateTime timestamp)
		{
			Name = name;
			Timestamp = timestamp;
		}

		public string Name;
		public DateTime Timestamp;
	}
}