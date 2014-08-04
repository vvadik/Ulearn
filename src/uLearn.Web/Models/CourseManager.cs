using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
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
				.OrderBy(f => GetSortingKey(f.Filename))
				.ToList();
			var includes = resourceFiles
				.Where(f => !IsSlide(f))
				.ToDictionary(inc => inc.Filename, inc => inc.GetContent().AsUtf8());
			var slides = resourceFiles
				.Where(IsSlide)
				.Select(f => LoadSlide(f, resourceFiles, name => includes[name]))
				.ToArray();
			var slidesWithRepeatedGuide =
				slides.GroupBy(x => x.Id).Where(x => x.Count() != 1).Select(x => x.Select(y => y.Info.CourseName + ": " + y.Title)).ToList();
			if (slidesWithRepeatedGuide.Any())
			{
				throw new Exception("change repeated guid in slides:\n" + string.Join("\n", slidesWithRepeatedGuide.Select(x => string.Join("\n", x))));
			}
			return new Course(courseId, courseTitle, slides);
		}

		private string GetSortingKey(string filename)
		{
			return filename.Split('-', '_', ' ')[0];
		}

		private static bool IsSlide(ResourceFile x)
		{
			return !x.Filename.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) && !x.Filename.Contains("._");
		}

		private static Slide LoadSlide(ResourceFile slideFile, IList<ResourceFile> resourceFiles, Func<string, string> getInclude)
		{
			try
			{
				var sourceCode = Encoding.UTF8.GetString(slideFile.GetContent());
				var usings = GetUsings(slideFile, resourceFiles); 
				var info = GetInfoForSlide(slideFile, resourceFiles);
				if (slideFile.Filename.EndsWith(".xml"))
				{
					var a = new XmlSerializer(typeof (Quiz));
					var quiz = (Quiz) a.Deserialize(new MemoryStream(slideFile.GetContent()));
					foreach (var quizBlock in quiz.QuizBlocks)
					{
						quizBlock.Text = Md.ToHtml(quizBlock.Text);
					}
					return new QuizSlide(new List<SlideBlock>(), info, quiz);
				}
				return SlideParser.ParseCode(sourceCode, info, usings, getInclude);
			}
			catch (Exception e)
			{
				throw new Exception("Error loading slide " + slideFile.FullName, e);
			}
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