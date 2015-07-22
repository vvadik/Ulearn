using System.IO;
using System.Linq;
using uLearn;

namespace uLearnToEdx
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("BasicProgramming.zip");
			var course = cm.GetCourses().Single();
			var converted = Converter.ToEdxCourse(course, "Kontur", new[] { "lti" }, new[] { "myname:rfe:qwerty" }, "192.168.33.1:44300");
			converted.Save();
			ArchiveManager.CreateTar(course.Id + ".tar.gz", course.Id);
		}
	}
}
