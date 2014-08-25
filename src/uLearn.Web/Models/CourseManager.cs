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
			result.AddCourse("Linq", "Практикум по LINQ");
			result.AddCourse("BasicProgramming", "Основы программирования на C#");
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
				.ThenBy(f => GetSecondSortingKey(f.Filename))
				.ToList();
			var includes = resourceFiles
				.Where(IsInclude)
				.ToDictionary(inc => inc.Filename, inc => inc.GetContent().AsUtf8());
			var slides = resourceFiles
				.Where(IsSlide)
				.Select((f, index) => LoadSlide(index, f, resourceFiles, name => includes[name]))
				.ToArray();
			CheckDuplicateSlideIds(slides);
			var notes = resourceFiles
				.Where(IsInstructorNote)
				.Select(f => LoadInstructorNote(courseId, f, resourceFiles))
				.ToArray();
			return new Course(courseId, courseTitle, slides, notes);
		}

		private static InstructorNote LoadInstructorNote(string courseId, ResourceFile resource, IEnumerable<ResourceFile> resourceFiles)
		{
			var content = resource.GetUtf8Content();
			var unitName = GetUnitName(resourceFiles, resource);
			return new InstructorNote(content, courseId, unitName);
		}

		private static void CheckDuplicateSlideIds(IEnumerable<Slide> slides)
		{
			var slidesWithDuplicateGuids =
				slides.GroupBy(x => x.Id)
					.Where(x => x.Count() != 1)
					.Select(x => x.Select(y => y.Title))
					.ToList();
			if (slidesWithDuplicateGuids.Any())
				throw new Exception("Duplecate Id in slides:\n" +
									string.Join("\n", slidesWithDuplicateGuids.Select(x => string.Join("\n", x))));
		}

		private string GetSecondSortingKey(string filename)
		{
			return filename.Split('.')[1].Split('_')[0];
		}

		private string GetSortingKey(string filename)
		{
			return filename.Split('-', '_', ' ')[0];
		}

		private static bool IsSlide(ResourceFile x)
		{
			var slideExtensions = new[] { ".cs", ".xml" };
			return slideExtensions.Any(ext => x.Filename.EndsWith(ext, StringComparison.OrdinalIgnoreCase)) && !IsInclude(x);
		}
		private static bool IsInstructorNote(ResourceFile x)
		{
			return x.Filename.EndsWith(".InstructorNotes.md", StringComparison.OrdinalIgnoreCase);
		}
		private static bool IsInclude(ResourceFile x)
		{
			return x.Filename.Contains("._");
		}

		private static Slide LoadSlide(int index, ResourceFile slideFile, IList<ResourceFile> resourceFiles, Func<string, string> getInclude)
		{
			try
			{
				var sourceCode = Encoding.UTF8.GetString(slideFile.GetContent());
				var prelude = GetExercisePrelude(slideFile, resourceFiles);
				var info = new SlideInfo(GetUnitName(resourceFiles, slideFile), index);
				return slideFile.Filename.EndsWith(".xml") 
					? LoadQuiz(slideFile, info) 
					: SlideParser.ParseCode(sourceCode, info, prelude, getInclude);
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
			var questionIndex = 1;
			var allReservedId = new HashSet<string>(quiz.Blocks.Select(x => x.Id));
			foreach (var quizBlock in quiz.Blocks)
			{
				var questionBlock = quizBlock as AbstractQuestionBlock;
				if (questionBlock != null)
					questionBlock.QuestionIndex = questionIndex++;
				if (quizBlock.Id == null)
				{
					while (allReservedId.Contains(emptyIndex.ToString()))
						emptyIndex++;
					quizBlock.Id = emptyIndex.ToString();
					emptyIndex++;
				}
				var choiceBlock = quizBlock as ChoiceBlock;
				if (choiceBlock == null) continue;
				var itemEmptyId = 0;
				var allReservedItemId = new HashSet<string>(choiceBlock.Items.Select(x => x.Id));
				foreach (var item in choiceBlock.Items.Where(item => item.Id == null))
				{
					while (allReservedItemId.Contains(itemEmptyId.ToString()))
						itemEmptyId++;
					item.Id = itemEmptyId.ToString();
					itemEmptyId++;
				}
			}
		}

		private static string GetExercisePrelude(ResourceFile file, IList<ResourceFile> all)
		{
			var detailedPath = file.FullName.Split('.').ToArray();
			return Encoding.UTF8.GetString(all.Single(x => x.FullName.EndsWith(detailedPath[detailedPath.Length - 3] + ".Prelude.txt")).GetContent());
		}

		private static string GetUnitName(IEnumerable<ResourceFile> all, ResourceFile resource)
		{
			var slidePath = resource.FullName.Split('.').ToArray();
			var fullName = string.Join(".", slidePath.Take(slidePath.Length - 2)) + ".Title.txt";
			return all.First(x => x.FullName == fullName).GetUtf8Content();
			
		}
	}
}