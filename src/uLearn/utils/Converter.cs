using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
using uLearn.Lessions;

namespace uLearn.utils
{

	public class Converter
	{
		[Test]
		[Explicit]
		public void CovertLessonSlidesToXml()
		{
			var coursesDirectory = new DirectoryInfo("../../../courses");
			var courseDirectories = coursesDirectory.GetDirectories("Slides", SearchOption.AllDirectories);
			var serializer = new XmlSerializer(typeof(Lesson));
			foreach (var courseDirectory in courseDirectories)
			{
				var course = new CourseLoader().LoadCourse(courseDirectory);
				foreach (var slide in course.Slides)
				{
					if (slide.ShouldBeSolved) continue;
					var lesson = new Lesson(slide.Title, slide.Id, slide.Blocks);
					var path = Path.ChangeExtension(slide.Info.SlideFile.FullName, "lesson.xml");
					var file = new FileInfo(path);
					using (var stream = file.OpenWrite())
						serializer.Serialize(stream, lesson);
					slide.Info.SlideFile.Delete();
				}
			}
		}
	}
}