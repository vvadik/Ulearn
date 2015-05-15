using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using UI.Tests.PageObjects;

namespace UI.Tests.TestCases.ULearn
{
	[TestFixture]
	public class ExercisePageShould
	{
		[Explicit]
		[Test]
		public void RunSolution()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnUrls.StartPage);
				UlearnDriver ulearnDriver = new UlearnDriver(driver);
				ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				driver.Navigate().GoToUrl("https://localhost:44300/Course/BasicProgramming/Slide/21");
			}
			
		}
	}
}
