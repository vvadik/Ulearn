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
				IUlearnDriver uDriver = new UlearnDriver(driver);
				uDriver.GoToRegistrationPage();
				var regPage = uDriver.GetPage() as RegistrationPage;
				regPage.SignUp(login, password);
				var startPage = uDriver.GetPage() as StartPage;
				startPage.GoToCourse(Titles.BasicProgrammingTitle);
				var slidePage = uDriver.GetPage() as SlidePage;
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
						yield return new TestCaseData(uDriver.GetPage(), courseName, unitName, slideIndex)
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
			var realPage = UlearnDriver.courseManager.GetCourse(courseName).Slides.First(x => x.Index == slideIndex);
			var blocks = page.Blocks.Where(NotUserCodeBlock).ToArray();
			foreach (var b in realPage.Blocks.Select((x, i) => new {Index = i, Value = x}))
			{
				if (b.Value is YoutubeBlock)
				{
					Assert.IsTrue(blocks[b.Index] is SlidePageVideoBlock);
				}
				if (b.Value is MdBlock)
				{
					Assert.AreEqual(string.Join("\n", (b.Value as MdBlock).Markdown).RenderMd(), (blocks[b.Index] as SlidePageTextBlock).Text);
				}
				if (b.Value is TexBlock)
				{
					
					//Assert.AreEqual((b.Value as MdBlock).Markdown, blocks[b.Index].Text);
				}
			}
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
