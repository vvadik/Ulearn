using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriverComponents;
using Selenium.UlearnDriverComponents.Pages;

namespace Selenium.SeleniumTests
{
	[TestFixture]
	class ContentPageShould
	{
		[Test]
		public void ClickNextButton()//тест не будет работать, если слайдов в курсе меньше 2х!
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				ulearnDriver = ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				var toc = ulearnDriver.GetToc();
				if (!toc.IsCollapsed(toc.GetUnitsName().First()))
					toc.GetUnitControl(toc.GetUnitsName().First()).Click();
				toc = ulearnDriver.GetToc();
				var unit = toc.GetUnitControl(toc.GetUnitsName().First());
				unit.GetSlides().First().Click();

				var slides = UlearnDriver.courseManager
						.GetCourses()
						.First(x => x.Title == "Основы программирования")
						.Slides.ToList();
				var currentSlideName = ulearnDriver.GetCurrentSlideName();
				var nextSlideName = "";
				for (var i = 0; i < slides.Count - 1; i++)
					if (slides[i].Title == currentSlideName)
					{
						nextSlideName = slides[i + 1].Title;
						break;
					}

				var page = ulearnDriver.GetPage() as UlearnContentPage;
				if (!page.IsActiveNextButton())
					(page as SlidePage).RateSlide(Rate.Trivial);
				ulearnDriver = page.ClickNextButton();
				Assert.AreEqual(nextSlideName, ulearnDriver.GetCurrentSlideName());
			}
		}
	}
}
