using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using uLearn.CSharp;
using uLearn.Model;

namespace uLearn.Utilities
{
	public class LessonToXmlConvertor
	{
		private readonly XmlSerializer lessonSerializer = new XmlSerializer(typeof(Lesson));

		[Test]
		public void ConvertSlidesFromDirectory()
		{
			var slidesDirectory = new DirectoryInfo(@"c:\work\edu\oop\OOP\OOP\Slides\L130 - FluentAPI");
			foreach (var slideFile in slidesDirectory.GetFiles("S*.cs"))
			{
				var slide = new CSharpSlideLoader().Load(slideFile, "unit", 0, CourseSettings.DefaultSettings);
				ConvertSlide(slide);
			}
		}

		[Test]
		[Explicit]
		public void CovertLessonSlidesToXml()
		{
			var coursesDirectory = new DirectoryInfo(@"c:\work\edu\oop\OOP\OOP\");
			var courseDirectories = coursesDirectory.GetDirectories("Slides", SearchOption.AllDirectories);
			foreach (var courseDirectory in courseDirectories)
			{
				var course = new CourseLoader().LoadCourse(courseDirectory);
				Console.WriteLine($"course {course.Id}");
				foreach (var slide in course.Slides)
				{
					ConvertSlide(slide);
				}
			}
		}

		private void ConvertSlide(Slide slide)
		{
			if (slide.ShouldBeSolved)
				return;
			Console.WriteLine(slide.Info.SlideFile.FullName);
			var lesson = new Lesson(slide.Title, slide.NormalizedGuid, slide.Blocks);
			var path = Path.ChangeExtension(slide.Info.SlideFile.FullName, "lesson.xml");
			using (var writer = new StreamWriter(path, false, Encoding.UTF8))
				lessonSerializer.Serialize(writer, lesson);
			slide.Info.SlideFile.Delete();
		}
	}
}