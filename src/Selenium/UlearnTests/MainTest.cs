using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriverComponents;
using Selenium.UlearnDriverComponents.Interfaces;
using Selenium.UlearnDriverComponents.PageObjects;
using Selenium.UlearnDriverComponents.Pages;
using uLearn;

namespace Selenium.UlearnTests
{
	[TestFixture]
	public class MainTest
	{
		[TestCaseSource("TestUlearn")]
		public void TestSlides(UlearnPage page, string courseName, string unitName, int slideIndex)
		{
			TestSlide(page, courseName, unitName, slideIndex);
		}

		public IEnumerable<TestCaseData> TestUlearn()
		{
			var r = new Random();
			var login = r.Next().ToString();
			var password = r.Next().ToString();
			const string courseName = "BasicProgramming";
			using (IWebDriver driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				UlearnDriver uDriver = new UlearnDriver(driver);
				var regPage = uDriver.GoToRegistrationPage();
				var startPage = regPage.SignUp(login, password);
				var slidePage = startPage.GoToCourse(Titles.BasicProgrammingTitle);
				if (slidePage.IsUserFirstVisit)
					slidePage.SelectGroup("group");
				var slideIndex = -1;
				foreach (var unitName in uDriver.GetToc().GetUnitsName().Take(1))
				{
					var unit = uDriver.GetToc().GetUnitControl(unitName);
					if (!uDriver.GetToc().IsCollapsed(unitName))
						unit.Click();
					foreach (var slide in unit.GetSlides().Take(5))
					{
						slide.Click();
						slideIndex++;
						yield return new TestCaseData(uDriver.Get<UlearnPage>(), courseName, unitName, slideIndex)
							.SetName(courseName + " " + unitName + " " + slide.Name);
					}
				}
			}
		}

		private static void TestSlide(UlearnPage page, string courseName, string unitName, int slideIndex)
		{
			if (page is ExerciseSlidePage)
				TestExercisePage(page as ExerciseSlidePage, courseName, unitName, slideIndex);
			else if (page is QuizSlidePage)
				TestQuizPage(page as QuizSlidePage, courseName, unitName, slideIndex);
			else if (page is SlidePage)
				TestSlidePage(page as SlidePage, courseName, unitName, slideIndex);
			else
				Assert.Fail("page is not a SlidePage");
		}

		private static void TestQuizPage(QuizSlidePage page, string courseName, string unitName, int slideIndex)
		{
			TestSlidePage(page, courseName, unitName, slideIndex);
			return;
		}

		private static void TestExercisePage(ExerciseSlidePage page, string courseName, string unitName, int slideIndex)
		{
			TestSlidePage(page, courseName, unitName, slideIndex);
			return;
		}

		private static void TestSlidePage(SlidePage page, string courseName, string unitName, int slideIndex)
		{
			
		}

		private static bool NotUserCodeBlock(SlidePageBlock b)
		{
			if (!(b is SlidePageCodeBlock))
				return true;
			if ((b as SlidePageCodeBlock).IsUserCode)
				return false;
			return true;
		}


		
	}
}
