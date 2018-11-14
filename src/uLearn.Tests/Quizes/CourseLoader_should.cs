using System.IO;
using NUnit.Framework;
using uLearn.Courses;

namespace uLearn.Quizes
{
	[TestFixture]
	public class CourseLoader_should
	{
		[Test, Explicit("Для профилирования загрузки курса")]
		public void LoadCourse()
		{
			new CourseLoader().LoadCourse(new DirectoryInfo(@"c:\work\edu\BasicProgramming\Part01\BasicProgramming\Slides"));
		}
	}
}