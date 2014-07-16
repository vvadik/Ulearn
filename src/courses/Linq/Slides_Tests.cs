using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using uLearn.Courses.Linq.Slides;

namespace uLearn.Courses.Linq
{
	[TestFixture]
	public class Slides_Tests
	{
		[TestCaseSource("GetSlideTests")]
		public void Slide(Type slideType)
		{
			SlideTestsUtils.TestExercise(slideType.GetMethod("Main"));
		}

		public IEnumerable<TestCaseData> GetSlideTests()
		{
			return Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t.Namespace == typeof (SelectWhereToArray).Namespace)
				.Where(t => SlideTestsUtils.GetExpectedOutputAttributes(t).Any())
				.Select(t => new TestCaseData(t));
		}
	}
}