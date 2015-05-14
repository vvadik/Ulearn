using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using UI.Tests.PageObjects;
using UI.Tests.PageObjects.PageObjects;
using UI.Tests.PageObjects.Pages;

namespace UI.Tests.TestCases.ULearn
{
	[TestFixture]
	public class MainEngineTest
	{
		private static readonly Dictionary<int, Action<UlearnDriver, SlidePage>> TestFactory = new Dictionary<int, Action<UlearnDriver, SlidePage>>
		{
			{0, TestTextBlocks},
			{1, TestGoodTex},
			{2, TestWrongTex},
			{3, TestVideoBlock},
			{4, TestCodeBlock},
			{5, TestExerciseSlidePage},
			{6, TestQuizSlidePage}
		};

		private static void TestQuizSlidePage(UlearnDriver driver, SlidePage page)
		{
			return;
		}

		private static void TestExerciseSlidePage(UlearnDriver driver, SlidePage page)
		{
			return;
		}

		private static void TestCodeBlock(UlearnDriver driver, SlidePage page)
		{
			Assert.IsTrue(page.Blocks.First() is SlidePageCodeBlock);
			Assert.AreEqual(1, page.Blocks.Count);
		}

		private static void TestVideoBlock(UlearnDriver driver, SlidePage page)
		{
			Assert.IsTrue(page.Blocks.First() is SlidePageVideoBlock);
			Assert.AreEqual(1, page.Blocks.Count);
		}

		private static void TestWrongTex(UlearnDriver driver, SlidePage page)
		{
			Assert.IsTrue(driver.TeX.All(x => !x.IsRendered));
		}

		private static void TestGoodTex(UlearnDriver driver, SlidePage page)
		{
			Assert.IsTrue(driver.TeX.All(x => x.IsRendered));
		}

		private static void TestTextBlocks(UlearnDriver driver, SlidePage page)
		{
			Assert.AreEqual(1, page.Blocks.Count);
			Assert.AreEqual("Параграф 1\r\nПараграф 2\r\nПараграф 3", ((SlidePageTextBlock)page.Blocks.First()).Text);
		}


		[Test]
		public void EnumeratePages()
		{
			var exceptions = new List<Exception>();
			var screenshotsPath = new List<string>();
			var r = new Random();
			var login = r.Next().ToString();
			var password = r.Next().ToString();
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnUrls.StartPage);
				var uDriver = new UlearnDriver(driver);
				var regPage = uDriver.GoToRegistrationPage();
				regPage.SignUp(login, password);

				var pages = uDriver.EnumeratePages("ForTests", login, password);
				TestAllSlides(pages, uDriver, exceptions, screenshotsPath, driver);
			}
			Verdict(exceptions, screenshotsPath);
		}

		private static void TestAllSlides(IEnumerable<SlidePage> pages, UlearnDriver uDriver, List<Exception> exceptions,
			List<string> screenshotsPath, IWebDriver driver)
		{
			foreach (var test in pages
				.Select((x, i) => new {Value = x, Index = i}))
			{
				try
				{
					Console.WriteLine("Page #" + test.Index);
					TestFactory[test.Index](uDriver, test.Value);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					exceptions.Add(e);
					screenshotsPath.Add(UlearnDriver.SaveScreenshot(driver));
				}
			}
		}

		private static void Verdict(IReadOnlyList<Exception> exceptions, IReadOnlyList<string> screenshotsPath)
		{
			if (exceptions.Count == 0)
				return;
			var exceptionString = new StringBuilder();
			for (var i = 0; i < exceptions.Count; i++)
			{
				exceptionString.Append(exceptions[i]);
				exceptionString.Append("\r\nPath to screenshot:\r\n===============\r\n");
				exceptionString.Append(screenshotsPath[i]);
				exceptionString.Append("\r\n===============\r\n");
			}
			throw new Exception(exceptionString.ToString());
		}
	}
}
