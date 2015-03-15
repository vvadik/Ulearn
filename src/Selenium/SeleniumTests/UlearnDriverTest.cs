using System;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriverComponents;
using Selenium.UlearnDriverComponents.Interfaces;
using Selenium.UlearnDriverComponents.Pages;

namespace Selenium.SeleniumTests
{
	[TestFixture]
	class UlearnDriverTest
	{

		[Test]
		public void FirstTest()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				IUlearnDriver ulearnDriver = new UlearnDriver(driver);
				ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				for (int i = 0; i < 10; i++)
				{
					Console.WriteLine(ulearnDriver.GetPage().GetPageType());
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
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				IUlearnDriver ulearnDriver = new UlearnDriver(driver);
				ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				var toc = ulearnDriver.GetToc();
				var unitsNames = toc.GetUnitsName();

				foreach (var unitName in unitsNames)
				{
					ulearnDriver.GetToc().GetUnitControl(unitName).Click();
					var slidesNames = ulearnDriver.GetToc().GetUnitControl(unitName).GetSlidesName();
					foreach (var slideName in slidesNames)
					{
						ulearnDriver.GetToc().GetUnitControl(unitName).GetSlides().First(x => x.Name == slideName).Click();
					}
				}
			}
		}

		[Test]
		public void TestTocCurrentSlide()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				IUlearnDriver ulearnDriver = new UlearnDriver(driver);
				ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				Console.WriteLine(ulearnDriver.GetCurrentSlideName());
			}
		}
	}
}
