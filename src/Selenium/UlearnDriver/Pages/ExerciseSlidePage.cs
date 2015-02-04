using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using Selenium.UlearnDriver.PageObjects;

namespace Selenium.UlearnDriver.Pages
{
	class ExerciseSlidePage : SlidePage
	{
		private readonly IWebElement runSolutionButton;
		private readonly IWebElement resetButton;
		private readonly IWebElement hintsButton;
		private List<Hint> hints;

		public ExerciseSlidePage(IWebDriver driver)
			: base(driver)
		{
			CheckExerciseSlide(driver);
			runSolutionButton = driver.FindElement(ElementsClasses.RunSolutionButton);
			resetButton = driver.FindElement(ElementsClasses.ResetButton);
			hintsButton = driver.FindElement(ElementsClasses.GetHintsButton);
			hints = GetHints();
		}

		private List<Hint> GetHints()
		{
			var allHints = driver.FindElement(By.Id("hint-place")).FindElements(By.XPath("/div"));
			var localHints = new List<Hint>(allHints.Count);
			for (var i = 0; i < allHints.Count; i++)
			{
				var likeButton = allHints[i].FindElement(By.Id(String.Format("{0}likeHint", i)));
				var hint = allHints[i].FindElement(By.XPath("/p"));
				localHints[i] = new Hint(new LikeButton(likeButton), hint);
			}
			return localHints;
		}

		public string GetHintButtonText()
		{
			return hintsButton.Text;
		}
		//public ExerciseSlidePage(IWebDriver driver, string courseTitle)
		//	: base(driver, courseTitle)
		//{
		//	CheckExerciseSlide(driver);
		//}

		public bool HasMoreHints()
		{
			return hintsButton.Enabled;
		}

		public UlearnDriver ClickRunButton()
		{
			runSolutionButton.Click();
			return new UlearnDriver(driver);
		}

		public UlearnDriver ClickResetButton()
		{
			resetButton.Click();
			return new UlearnDriver(driver);
		}

		public UlearnDriver ClickHintsButton()
		{
			hintsButton.Click();
			return new UlearnDriver(driver);
		}

		public string GetTextFromSecretCode()
		{
			return driver.FindElement(ElementsId.SecreteCode).Text;
		}

		public string GetUserCodeText()
		{
			return driver.FindElement(ElementsClasses.CodeExercise).Text;
		}

		private static void CheckExerciseSlide(IWebDriver driver)
		{
			var secretCodeText = driver.FindElement(ElementsId.SecreteCode);
			if (secretCodeText == null)
			{
				throw new NotFoundException("не найдена секретная область кода");
			}
			var codeExerciseField = driver.FindElement(ElementsClasses.CodeExercise);
			if (codeExerciseField == null)
			{
				throw new NotFoundException("не найдена область для кода");
			}
			var codeMirrorObject = driver.FindElement(ElementsClasses.CodeMirror);
			if (codeMirrorObject == null)
			{
				throw new NotFoundException("не найден codemirror");
			}
		}
	}
}
