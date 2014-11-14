using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.PageObjects;

namespace Selenium.Tests
{
	[TestFixture]
	public class TestRateButton
	{
		private static readonly IWebDriver driver = new ChromeDriver();

		public SlidePage LoginAndGoToCourse(string courseTitle)
		{
			//IWebDriver driver = new ChromeDriver();
			driver.Navigate().GoToUrl(ULearnReferences.startPage);

			var startPage = new StartPage(driver);
			var signInPage = startPage.GoToSignInPage();
			var authorisedStartPage = signInPage.LoginValidUser(Admin.Login, Admin.Password);

			return authorisedStartPage.GoToCourse(Titles.BasicProgrammingTitle);
		}

		[Test]
		public void TestRate1()
		{
			using (driver)
			{
				var slide = LoginAndGoToCourse(Titles.BasicProgrammingTitle);
				slide.RateSlide(Rate.Trivial);
			}
		}
	}
}
