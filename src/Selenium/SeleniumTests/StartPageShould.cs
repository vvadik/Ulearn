using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriverComponents;
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
				var startPage = ulearnDriver.GetPage() as StartPage;
				Assert.AreEqual(PageType.StartPage, startPage.GetPageType());
			}
		}

		[Test]
		public void GoToSignInPage()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriverComponents.UlearnDriver(driver);
				var startPage = ulearnDriver.GetPage() as StartPage;

				ulearnDriver = startPage.GoToSignInPage();
				Assert.AreEqual(PageType.SignInPage, ulearnDriver.GetPage().GetPageType());

				ulearnDriver = ulearnDriver.GoToStartPage();
				var isLogin = ulearnDriver.IsLogin();
				Assert.IsFalse(isLogin);
			}
		}

		[Test]
		public void BeNotLogin()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriverComponents.UlearnDriver(driver);
				var startPage = ulearnDriver.GetPage() as StartPage;

				ulearnDriver = startPage.GoToSignInPage();

				ulearnDriver = ulearnDriver.GoToStartPage();
				var isLogin = ulearnDriver.IsLogin();
				Assert.IsFalse(isLogin);
			}
		}



		[Test]
		public void TestNavArrows()
		{
			using (var driver = new ChromeDriver())
			{
				//driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				//var ulearnDriver = new UlearnDriver.UlearnDriver(driver);
				//var startPage = ulearnDriver.GetPage() as StartPage;
				//ulearnDriver = startPage.GoToSignInPage();
				//var signInPage = ulearnDriver.GetPage() as SignInPage;
				//signInPage.
				//Console.WriteLine(ulearnDriver.GetPage().GetPageType());
			}
		}
	}
}
