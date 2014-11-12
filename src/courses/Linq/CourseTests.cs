using NUnit.Framework;
using uLearn.Courses.Linq.Slides.LINQ;

namespace uLearn.Courses.Linq
{
	[TestFixture]
	public class CourseTests : BaseCourseTests
	{
		public CourseTests() : base(typeof(S010_Intro))
		{
		}
	}
}