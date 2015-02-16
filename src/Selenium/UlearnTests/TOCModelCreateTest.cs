using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriver;
using Selenium.UlearnDriver.Pages;

namespace Selenium.Tests
{
	[TestFixture]
	class TOCModelCreateTest
	{
		private static readonly IWebDriver driver = new ChromeDriver();

		

		[Test]
		public void TestTOCCreate()
		{
			//var slide = LoginAndGoToCourse(Titles.BasicProgrammingTitle);

			//var TOC = slide.GetTOC();

			//Assert.AreNotEqual(null, TOC);
			
			driver.Dispose();
		}

		[Test]
		[Explicit]
		public void TestTOCNavigate()
		{
			//var slide = LoginAndGoToCourse(Titles.BasicProgrammingTitle);
			//var TOC = slide.GetTOC();
			using (driver)
			{
				//foreach (var unitName in TOC.GetUnitsNames())
				{
					//if (!slide.IsActiveNextButton())
					//	slide.RateSlide(Rate.Trivial);
					//slide = slide.ClickNextButton();
					//var unit = TOC.GetUnitControl(unitName);
					//var names = unit.GetSlidesNames();
					//TOC = unit.Click();
					//foreach (var slideName in names)
					//{
					//	TOC = TOC.GetUnitControl(unitName).ClickOnSlide(slideName);
					//}
				}
			}
		}
	}
}
