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
using uLearn;

namespace Selenium.SeleniumTests
{
	[TestFixture]
	public class TocShould
	{
		[Test]
		public void BeNotNullInCourse()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				ulearnDriver = ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				var toc = ulearnDriver.GetToc();
				Assert.IsNotNull(toc);
			}
		}

		[Test]
		[ExpectedException(typeof(NotFoundException))]
		public void BeNullInStartPage()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				ulearnDriver.GetToc();
				//ulearnDriver = (ulearnDriver.GetPage() as StartPage).GoToSignInPage();
				//Assert.IsNull(ulearnDriver.GetToc());
			}
		}

		[Test]
		[ExpectedException(typeof(NotFoundException))]
		public void BeNullInSignInPage()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				ulearnDriver = (ulearnDriver.GetPage() as StartPage).GoToSignInPage();
				ulearnDriver.GetToc();
			}
		}

		[Test]
		public void GetUnits()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				ulearnDriver = ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				var toc = ulearnDriver.GetToc();
				var unitsName = toc.GetUnitsName();
				var course = UlearnDriver.courseManager
					.GetCourse(UlearnDriver.courseManager
						.GetCourses()
						.First(x => x.Title == "Основы программирования").Id);
				var cmUunitsName = course.GetUnits().Distinct();
				CollectionAssert.AreEquivalent(unitsName, cmUunitsName);
			}
		}

		[Test]
		public void GetSlidesFromUnit()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				ulearnDriver = ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				var toc = ulearnDriver.GetToc();
				var unitsName = toc.GetUnitsName();
				foreach (var unitName in unitsName)
				{
					var unit = toc.GetUnitControl(unitName);
					if (!toc.IsCollapsed(unitName))
					{
						ulearnDriver = unit.Click();
					}
					var slides = ulearnDriver.GetToc().GetUnitControl(unitName).GetSlidesName();
					var course = UlearnDriver.courseManager
					.GetCourse(UlearnDriver.courseManager
						.GetCourses()
						.First(x => x.Title == "Основы программирования").Id);
					var cmSlides = course.Slides.Where(s => s.Info.UnitName == unitName).Select(x => x.Title);
					CollectionAssert.AreEquivalent(cmSlides, slides);
				}
			}
		}

		[Test]
		public void GetSlideType()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				ulearnDriver = ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				var toc = ulearnDriver.GetToc();
				var unitsName = toc.GetUnitsName();
				foreach (var unitName in unitsName)
				{
					var unit = toc.GetUnitControl(unitName);
					if (!toc.IsCollapsed(unitName))
					{
						ulearnDriver = unit.Click();
					}
					var slides = ulearnDriver.GetToc().GetUnitControl(unitName).GetSlides().ToList();
					var course = UlearnDriver.courseManager
					.GetCourse(UlearnDriver.courseManager
						.GetCourses()
						.First(x => x.Title == "Основы программирования").Id);
					var cmSlides = course.Slides.Where(s => s.Info.UnitName == unitName).ToList();
					for (var i = 0; i < slides.Count; i++)
					{
						var type = cmSlides[i] is ExerciseSlide ? PageType.ExerciseSlidePage :
							cmSlides[i] is QuizSlide ? PageType.QuizSlidePage :
								PageType.SlidePage;
						Assert.AreEqual(type, slides[i].SlideType);
					}
				}
			}
		}
	}
}
