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

namespace Selenium.SeleniumTests.TestSeleniumCourse
{
	[TestFixture]
	public class MainEngineTest
	{
		

		[Test]
		public void TestSlidePage()
		{
			using (IWebDriver driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var uDriver = new UlearnDriver(driver);
				uDriver.GoToRegistrationPage();
				var page = uDriver.Get<RegistrationPage>();
				page.SignUp("user", "asdasd");
				driver.Navigate().GoToUrl("https://ulearn.azurewebsites.net/Course/SampleCourse");
				var toc = uDriver.GetToc();
				toc.GetUnitControl(toc.GetUnitsName().First())
					.GetSlides()
					.First()
					.Click();
				var sPage = uDriver.Get<SlidePage>();
				uDriver.CheckTex();
				Assert.AreEqual(4, sPage.Blocks.Count);
			}
		}

		[Test]
		public void TestExerciseSlidePage()
		{
			using (IWebDriver driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var uDriver = new UlearnDriver(driver);
				uDriver.GoToRegistrationPage();
				var page = uDriver.Get<RegistrationPage>();
				page.SignUp("user", "asdasd");
				driver.Navigate().GoToUrl("https://ulearn.azurewebsites.net/Course/SeleniumCourse");
				var toc = uDriver.GetToc();
				toc.GetUnitControl(toc.GetUnitsName().First())
					.GetSlides()
					.Skip(1)
					.First()
					.Click();
				var sPage = uDriver.Get<SlidePage>();
				Assert.AreEqual(2, sPage.Blocks.Count);
			}
		}
	}
}
