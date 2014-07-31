using NUnit.Framework;
using uLearn.Courses.Linq.Slides;

namespace uLearn.Courses.Linq
{
	[TestFixture]
	public class Slides_Tests : BaseSlideTests
	{
		public Slides_Tests() : base(typeof(Intro))
		{
		}
	}
}