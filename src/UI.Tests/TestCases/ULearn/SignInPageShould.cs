using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using UI.Tests.PageObjects;
using UI.Tests.PageObjects.Pages;

namespace UI.Tests.TestCases.ULearn
{
	[TestFixture]
	public class SignInPageShould
	{
		[Test]
		public void LoginAsValidUser()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnUrls.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				var startPage = ulearnDriver.Get<StartPage>();
				var signInPage = startPage.GoToSignInPage();
				signInPage.LoginValidUser(Admin.Login, Admin.Password);
				Assert.AreEqual(Admin.Login, ulearnDriver.GetCurrentUserName());
			}
		}

		[Explicit]
		[Test]
		public void LoginVk()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnUrls.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				var startPage = ulearnDriver.Get<StartPage>();
				var signInPage = startPage.GoToSignInPage();
				signInPage.LoginVk();
				Assert.AreEqual(Admin.Login, ulearnDriver.GetCurrentUserName());
			}
		}
	}
}
