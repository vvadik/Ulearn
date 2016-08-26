using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn.Quizes
{
	[TestFixture]
	public class CourseLoader_should
	{
		[Test]
		public void LoadCourse()
		{
			new CourseLoader().LoadCourse(new DirectoryInfo(@"c:\work\edu\BasicProgramming\Part01\BasicProgramming\Slides"));
		}
	}
}
