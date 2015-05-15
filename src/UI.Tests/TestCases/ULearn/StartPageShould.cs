using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using UI.Tests.PageObjects;
using UI.Tests.PageObjects.Pages;

namespace UI.Tests.TestCases.ULearn
{
	[TestFixture]
	class StartPageShould
	{

		[Test]
		public void GetStartPage()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnUrls.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				var startPage = ulearnDriver.Get<StartPage>();
				Assert.AreEqual(PageType.StartPage, ulearnDriver.GetCurrentPageType());
			}
		}

		[Test]
		public void GoToSignInPage()
		{
			using (IWebDriver driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnUrls.StartPage);
				UlearnDriver ulearnDriver = new UlearnDriver(driver);
				var startPage = ulearnDriver.Get<StartPage>();

				startPage.GoToSignInPage();
				Assert.AreEqual(PageType.SignInPage, ulearnDriver.GetCurrentPageType());

				ulearnDriver.GoToStartPage();
				var isLogin = ulearnDriver.LoggedIn;
				Assert.IsFalse(isLogin);
			}
		}

		[Test]
		public void NotBeLoggedIn()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnUrls.StartPage);
				UlearnDriver ulearnDriver = new UlearnDriver(driver);
				var startPage = ulearnDriver.Get<StartPage>();

				startPage.GoToSignInPage();

				ulearnDriver.GoToStartPage();
				var isLogin = ulearnDriver.LoggedIn;
				Assert.IsFalse(isLogin);
			}
		}
	}
}
