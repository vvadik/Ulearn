using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using uLearn.CSharp;

namespace uLearn.Web.Models
{
	public class CourseManager
	{
		private static readonly ResourceLoader Resources = new ResourceLoader(typeof (CourseManager));

		public static CourseManager AllCourses = LoadAllCourses();

		public readonly List<Course> Courses = new List<Course>();


		public static CourseManager LoadAllCourses()
		{
			var result = new CourseManager();
			result.AddCourse("Linq", "Linq");
			result.AddCourse("BasicProgramming", "BasicProgramming");
			return result;
		}

		public Course GetCourse(string courseId)
		{
			Course course = Courses.FirstOrDefault(c => c.Id == courseId);
			if (course == null) throw new Exception(string.Format("Course {0} not found", courseId));
			return course;
		}

		public ExerciseSlide GetExerciseSlide(string courseId, int slideIndex)
		{
			Course course = GetCourse(courseId);
			return (ExerciseSlide)course.Slides[slideIndex];
		}


		private void AddCourse(string courseId, string courseTitle)
		{
			Courses.Add(LoadCourse(courseId, courseTitle));
		}

		private Course LoadCourse(string courseId, string courseTitle)
		{
			var resourceFiles = Resources.EnumerateResourcesFrom("uLearn.Web.Courses." + courseId)
				.OrderBy(f => f.Filename)
				.ToList();
			var includes = resourceFiles
				.Where(f => !IsSlide(f))
				.ToDictionary(inc => inc.Filename, inc => inc.GetContent().AsUtf8());
			var slides = resourceFiles
				.Where(IsSlide)
				.Select(f => LoadSlide(f, resourceFiles, name => includes[name]))
				.ToArray();
			return new Course(courseId, courseTitle, slides);
		}

		private static bool IsSlide(ResourceFile x)
		{
			return !x.Filename.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) && !x.Filename.Contains("._");
		}

		private static Slide LoadSlide(ResourceFile slideFile, IList<ResourceFile> resourceFiles, Func<string, string> getInclude)
		{
			var sourceCode = Encoding.UTF8.GetString(slideFile.GetContent());
			//var tmp = fileWithUsings.GetContent();
			var usings = GetUsings(slideFile, resourceFiles); 
			var info = GetInfoForSlide(slideFile, resourceFiles);
			return SlideParser.ParseCode(sourceCode, info, usings, getInclude);
		}

		private static string GetUsings(ResourceFile file, IList<ResourceFile> all)
		{
			var detailedPath = file.FullName.Split('.').ToArray();
			return Encoding.UTF8.GetString(all.Single(x => x.FullName.EndsWith(detailedPath[detailedPath.Length - 3] + ".Usings.txt")).GetContent());
		}

		private static SlideInfo GetInfoForSlide(ResourceFile file, IList<ResourceFile> all)
		{
			var detailedPath = file.FullName.Split('.').ToArray();
			return new SlideInfo(
				fileName: detailedPath[detailedPath.Length - 2] + "." + detailedPath.Last(),
				courseName: GetTitle(all, detailedPath, 3),
				unitName: GetTitle(all, detailedPath, 2)
				);
		}

		private static string GetTitle(IEnumerable<ResourceFile> all, string[] slidePath, int depth)
		{
			var fullName = string.Join(".", slidePath.Take(slidePath.Length - depth)) + ".Title.txt";
			return Encoding.UTF8.GetString(all.First(x => x.FullName == fullName).GetContent());
			
		}
	}
}