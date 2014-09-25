using NUnit.Framework;
using OOP.Slides.U00_Intro;
using uLearn;

namespace OOP
{
	[TestFixture]
	public class CoursesTests : BaseCourseTests
	{
		public CoursesTests() : base(typeof(S000_Intro))
		{
		}
	}
}