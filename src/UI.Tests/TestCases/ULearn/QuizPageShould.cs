using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using UI.Tests.PageObjects;
using UI.Tests.PageObjects.Pages;

namespace UI.Tests.TestCases.ULearn
{
	[TestFixture]
	public class QuizPageShould
	{
		[Explicit]
		[Test]
		public void CheckAnswers()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnUrls.StartPage);
				UlearnDriver ulearnDriver = new UlearnDriver(driver);
				ulearnDriver.GoToRegistrationPage();
				var registrationPage = ulearnDriver.Get<RegistrationPage>();
				ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				//var first
				var page = ulearnDriver.Get<SlidePage>();
			}
		}
	}
}
