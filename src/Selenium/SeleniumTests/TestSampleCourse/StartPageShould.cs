using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriverComponents;
using Selenium.UlearnDriverComponents.Interfaces;
using Selenium.UlearnDriverComponents.Pages;

namespace Selenium.SeleniumTests
{
	[TestFixture]
	class StartPageShould
	{

		[Test]
		public void GetStartPage()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriverComponents.UlearnDriver(driver);
				var startPage = ulearnDriver.Get<StartPage>();
				Assert.AreEqual(PageType.StartPage, ulearnDriver.GetCurrentPageType());
			}
		}

		[Test]
		public void GoToSignInPage()
		{
			using (IWebDriver driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				UlearnDriver ulearnDriver = new UlearnDriver(driver);
				var startPage = ulearnDriver.Get<StartPage>();

				startPage.GoToSignInPage();
				Assert.AreEqual(PageType.SignInPage, ulearnDriver.GetCurrentPageType());

				ulearnDriver.GoToStartPage();
				var isLogin = ulearnDriver.IsLogin;
				Assert.IsFalse(isLogin);
			}
		}

		[Test]
		public void BeNotLogin()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				UlearnDriver ulearnDriver = new UlearnDriver(driver);
				var startPage = ulearnDriver.Get<StartPage>();

				startPage.GoToSignInPage();

				ulearnDriver.GoToStartPage();
				var isLogin = ulearnDriver.IsLogin;
				Assert.IsFalse(isLogin);
			}
		}
	}
}
