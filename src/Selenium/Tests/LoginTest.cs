using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Selenium.PageObjects;

namespace Selenium.Tests
{
	[TestFixture]
	public class LoginTest
	{
		[Test]
		public void TestLoginValidUser()
		{
			IWebDriver driver = new ChromeDriver();
			driver.Navigate().GoToUrl(ULearnReferences.startPage);

			var startPage = new StartPage(driver);
			var signInPage = startPage.GoToSignInPage();
			signInPage.LoginValidUser(Admin.login, Admin.Password);

			driver.Dispose();
		}

		[Test]
		public void TestLoginVK()
		{
			IWebDriver driver = new ChromeDriver();
			driver.Navigate().GoToUrl(ULearnReferences.startPage);

			var startPage = new StartPage(driver);
			var signInPage = startPage.GoToSignInPage();

			try
			{
				signInPage.LoginVk();
			}
			catch (IllegalLocatorException)
			{
				driver.Dispose();
			}
		}

		[Test]
		public void TestGoToCourseBP()
		{
			IWebDriver driver = new ChromeDriver();
			driver.Navigate().GoToUrl(ULearnReferences.startPage);

			var startPage = new StartPage(driver);
			var signInPage = startPage.GoToSignInPage();
			var authorisedStartPage = signInPage.LoginValidUser(Admin.login, Admin.Password);

			authorisedStartPage.GoToCourse(Titles.BasicProgrammingTitle);

			driver.Dispose();
		}

		[Test]
		public void TestGoToCourseLinq()
		{
			IWebDriver driver = new ChromeDriver();
			driver.Navigate().GoToUrl(ULearnReferences.startPage);

			var startPage = new StartPage(driver);
			var signInPage = startPage.GoToSignInPage();
			var authorisedStartPage = signInPage.LoginValidUser(Admin.login, Admin.Password);

			authorisedStartPage.GoToCourse(Titles.LinqTitle);

			driver.Dispose();
		}
	}
}
