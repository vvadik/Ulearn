using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriverComponents;
using Selenium.UlearnDriverComponents.PageObjects;
using Selenium.UlearnDriverComponents.Pages;

namespace Selenium.SeleniumTests.TestSampleCourse
{
	public class TestData
	{
		public TestData(Action<UlearnDriver> test, string testName)
		{
			Test = test;
			TestName = testName;
		}

		public string TestName { get; private set; }

		public Action<UlearnDriver> Test { get; private set; }
	}

	[TestFixture]
	public class MainEngineTest
	{
		private static readonly Dictionary<int, TestData> TestFactory = new Dictionary<int, TestData>
		{
			{0, new TestData(TestTextBlocks, "Text_blocks")},
			{1, new TestData(TestGoodTex, "Good_tex")},
			{2, new TestData(TestWrongTex, "Wrong_tex")},
			{3, new TestData(TestVideoBlock, "Video_block")},
			{4, new TestData(TestCodeBlock, "Code_block")},
			{5, new TestData(TestExerciseSlidePage, "ExerciseSlidePage")},
			{6, new TestData(TestQuizSlidePage, "QuizSlidePage")},
		};

		private ChromeDriver driver;

		private static void TestQuizSlidePage(UlearnDriver obj)
		{
			return;
		}

		private static void TestExerciseSlidePage(UlearnDriver obj)
		{
			return;
		}

		private static void TestCodeBlock(UlearnDriver driver)
		{
			Assert.IsTrue(driver.Get<SlidePage>().Blocks.First() is SlidePageCodeBlock);
			Assert.AreEqual(1, driver.Get<SlidePage>().Blocks.Count);
		}

		private static void TestVideoBlock(UlearnDriver driver)
		{
			Assert.IsTrue(driver.Get<SlidePage>().Blocks.First() is SlidePageVideoBlock);
			Assert.AreEqual(1, driver.Get<SlidePage>().Blocks.Count);
		}

		private static void TestWrongTex(UlearnDriver driver)
		{
			Assert.IsFalse(driver.TeX.All(x => x.IsRender));
		}

		private static void TestGoodTex(UlearnDriver driver)
		{
			Assert.IsTrue(driver.TeX.All(x => x.IsRender));
		}

		private static void TestTextBlocks(UlearnDriver driver)
		{
			var page = driver.Get<SlidePage>();
			Assert.AreEqual(1, page.Blocks.Count);
			Assert.AreEqual("Параграф 1\r\nПараграф 2\r\nПараграф 3", (page.Blocks.First() as SlidePageTextBlock).Text);
		}


		[Test]
		[TestCaseSource("EnumeratePages")]
		public void TestSampleSlides(TestData testData, UlearnDriver uDriver, string url)
		{
			uDriver.GoToUrl(url);
			testData.Test(uDriver);
		}

		public IEnumerable<TestCaseData> EnumeratePages()
		{
			var r = new Random();
			var login = r.Next().ToString();
			var password = r.Next().ToString();
			driver = new ChromeDriver();
			driver.Navigate().GoToUrl(ULearnReferences.StartPage);
			var uDriver = new UlearnDriver(driver);
			var regPage = uDriver.GoToRegistrationPage();
			regPage.SignUp(login, password);

			return uDriver
				.EnumeratePages("SampleCourse", login, password)
				.Select((x, i) => new TestCaseData(TestFactory[i], uDriver, uDriver.Url).SetName(TestFactory[i].TestName));
		}
	}
}
