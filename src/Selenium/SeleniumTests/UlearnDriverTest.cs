using System;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriver;
using Selenium.UlearnDriver.Pages;
using uLearn.Web.DataContexts;
using Selenium.UlearnDriver;

namespace SeleniumTests
{
	[TestFixture]
	class UlearnDriverTest
	{
		private static readonly IWebDriver driver = new ChromeDriver();

		[Test]
		public void FirstTest()
		{
			using (driver)
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				ulearnDriver = ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				for (int i = 0; i < 10; i++)
				{
					var page = ulearnDriver.GetPage() as UlearnContentPage;
					if (!page.IsActiveNextButton())
						(page as SlidePage).RateSlide(Rate.Trivial);
					ulearnDriver = page.ClickNextButton();
				}
			}
		}

		[Test]
		public void TestToc()
		{
			using (driver)
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				ulearnDriver = ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				var toc = ulearnDriver.GetToc();
				var unitsNames = toc.GetUnitsNames();

				foreach (var unitName in unitsNames)
				{
					ulearnDriver = ulearnDriver.GetToc().GetUnitControl(unitName).Click();
					var slidesNames = ulearnDriver.GetToc().GetUnitControl(unitName).GetSlidesNames();
					foreach (var slideName in slidesNames)
					{
						ulearnDriver = ulearnDriver.GetToc().GetUnitControl(unitName).ClickOnSlide(slideName);
					}
				}
			}
		}
	}
}
