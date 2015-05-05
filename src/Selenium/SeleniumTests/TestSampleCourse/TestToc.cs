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
	class TestToc
	{
		[Explicit]
		[Test]
		public void TestUnits()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				UlearnDriver ulearnDriver = new UlearnDriver(driver);
				var regPage = ulearnDriver.GoToRegistrationPage();
				var random = new Random();
				var login = random.Next().ToString();
				var password = random.Next().ToString();
				regPage.SignUp(login, password);
				var startPage = ulearnDriver.GoToStartPage();
				startPage.GoToCourse(Titles.BasicProgrammingTitle);
				var toc = ulearnDriver.GetToc();
				var unitsNames = toc.GetUnitsName();

				foreach (var unitName in unitsNames)
				{
					if (!ulearnDriver.GetToc().GetUnitControl(unitName).Collapse)
						ulearnDriver.GetToc().GetUnitControl(unitName).Click();
					var slidesNames = ulearnDriver.GetToc().GetUnitControl(unitName).GetSlidesName();
					foreach (var slideName in slidesNames)
					{
						ulearnDriver.GetToc().GetUnitControl(unitName).GetSlides().First(x => x.Name == slideName).Click();
					}
				}
			}
		}
	}
}
