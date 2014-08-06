using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using uLearn.CSharp;
using uLearn.Quizes;

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
				var usings = GetExercisePrelude(slideFile, resourceFiles); 
				var info = GetInfoForSlide(slideFile, resourceFiles);
				return slideFile.Filename.EndsWith(".xml") 
					? LoadQuiz(slideFile, info) 
					: SlideParser.ParseCode(sourceCode, info, usings, getInclude);
			}
			catch (Exception e)
			{
				throw new Exception("Error loading slide " + slideFile.FullName, e);
			}
		}

		private static Slide LoadQuiz(ResourceFile slideFile, SlideInfo info)
		{
			var serializer = new XmlSerializer(typeof (Quiz));
			var quiz = (Quiz) serializer.Deserialize(new MemoryStream(slideFile.GetContent()));
			FillEmptyField(quiz);
			return new QuizSlide(info, quiz);
		}

		private static void FillEmptyField(Quiz quiz)
		{
			var emptyIndex = 0;
			foreach (var quizBlock in quiz.Blocks)
			{
				quizBlock.Text = Md.ToHtml(quizBlock.Text);
				if (quizBlock.Id == null)
				{
					quizBlock.Id = emptyIndex.ToString();
					emptyIndex++;
				}
				var choiceBlock = quizBlock as ChoiceBlock;
				if (choiceBlock == null) continue;
				for (var itemIndex = 0; itemIndex < choiceBlock.Items.Length; itemIndex++)
					if (choiceBlock.Items[itemIndex].Id == null)
						choiceBlock.Items[itemIndex].Id = itemIndex.ToString();
				//if (!choiceBlock.Shuffle) continue;
				//Shuffle(choiceBlock.Items);
			}
		}

		//private static void Shuffle(ChoiceItem[] items)
		//{
		//	var shuffledItems = new ChoiceBlock[items.Length];
		//}

		private static string GetExercisePrelude(ResourceFile file, IList<ResourceFile> all)
		{
			var detailedPath = file.FullName.Split('.').ToArray();
			return Encoding.UTF8.GetString(all.Single(x => x.FullName.EndsWith(detailedPath[detailedPath.Length - 3] + ".Prelude.txt")).GetContent());
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