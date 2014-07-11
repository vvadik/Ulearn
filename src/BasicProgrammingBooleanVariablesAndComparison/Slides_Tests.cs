using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using uLearn.Courses.BasicProgrammingIfWhileFor.Slides;

namespace uLearn.Courses.BasicProgramming
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
				.Where(t => t.Namespace == typeof(S01_BooleanVariablesAndComparison).Namespace)
				.Where(t => SlideTestsUtils.GetExpectedOutputAttributes(t).Any())
				.Select(t => new TestCaseData(t));
		}
	}
}