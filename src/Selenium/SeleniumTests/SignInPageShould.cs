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
	public class SignInPageShould
	{
		[Test]
		public void LoginAsValidUser()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
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
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				var startPage = ulearnDriver.Get<StartPage>();
				var signInPage = startPage.GoToSignInPage();
				signInPage.LoginVk();
				Assert.AreEqual(Admin.Login, ulearnDriver.GetCurrentUserName());
			}
		}
	}
}
