using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UI.Tests.PageObjects;

namespace UI.Tests.TestCases.Courses
{
	[TestFixture]
	public class TeX_Should
	{
		[Explicit]
		[TestCase("BasicProgramming")]
		[TestCase("BasicProgramming2")]
		[TestCase("LINQ")]
		public void beRendered_onAllPages(string courseTitle)
		{
			var exceptions = FindTexErrors(courseTitle).ToList();
			if (exceptions.Any())
				throw new Exception(string.Join("\n", exceptions));
		}

		private static IEnumerable<string> FindTexErrors(string courseId)
		{
			using (var driver = new UlearnDriver())
			{
				driver.GoToRegistrationPage().SignUpAsRandomUser();
				return (
					from page in driver.EnumeratePages(courseId)
					from tex in driver.TeX
					where !tex.IsRendered
					select page.GetSlideName() + " TeX is not rendered! " + tex.GetContent()
					).ToList();
			}
		}
	}
}
