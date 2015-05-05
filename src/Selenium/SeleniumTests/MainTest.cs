using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Selenium.UlearnDriverComponents;

namespace Selenium.SeleniumTests
{
	[TestFixture]
	public class MainTest
	{
		[Explicit]
		[Test]
		public void CheckTeX()
		{
			var exceptions = new List<Exception>();
			var screenshotsPath = new List<string>();
			using (var driver = new UlearnDriver())
			{
				var regPage = driver.GoToRegistrationPage();
				var startPage = regPage.SignUpAsRandomUser();
				startPage.GoToCourse(Titles.BasicProgrammingTitle);
				foreach (var page in driver.EnumeratePages(Titles.BasicProgrammingTitle))
				{
					try
					{
						Assert.IsTrue(driver.TeX.All(x => x.IsRender));
					}
					catch(Exception e)
					{
						exceptions.Add(e);
						screenshotsPath.Add(driver.SaveScreenshot());
					}
				}
			}
			Verdict(exceptions, screenshotsPath);
		}

		private static void Verdict(IReadOnlyList<Exception> exceptions, IReadOnlyList<string> screenshotsPath)
		{
			if (exceptions.Count == 0)
				return;
			var exceptionString = new StringBuilder();
			for (var i = 0; i < exceptions.Count; i++)
			{
				exceptionString.Append(exceptions[i]);
				exceptionString.Append("\r\nPath to screenshot:\r\n===============\r\n");
				exceptionString.Append(screenshotsPath[i]);
				exceptionString.Append("\r\n===============\r\n");
			}
			throw new Exception(exceptionString.ToString());
		}
	}
}
