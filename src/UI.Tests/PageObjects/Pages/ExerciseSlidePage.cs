using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using UI.Tests.PageObjects.PageObjects;

namespace UI.Tests.PageObjects.Pages
{
	class ExerciseSlidePage : SlidePage
	{
		private IWebElement runSolutionButton;
		private IWebElement resetButton;
		private IWebElement hintsButton;

		public ExerciseSlidePage(IWebDriver driver)
			: base(driver)
		{
			Configure();
		}

		private new void Configure()
		{
			base.Configure();
			runSolutionButton = driver.FindElement(ElementsClasses.RunSolutionButton);
			resetButton = driver.FindElement(ElementsClasses.ResetButton);
			hintsButton = driver.FindElement(ElementsClasses.GetHintsButton);
			//CheckExerciseSlide();
		}

		private List<Hint> GetHints()
		{
			const string hintXpath = "hyml/body/div[1]/div/div/div/div[9]/div/div";
			var allHints = UlearnDriver.FindElementsSafely(driver, By.XPath(hintXpath));
			// driver.FindElement(By.Id("hint-place")).FindElements();
			var localHints = new List<Hint>(allHints.Count);
			for (var i = 0; i < allHints.Count; i++)
			{
				var likeButton = allHints[i].FindElement(By.Id(String.Format("{0}likeHint", i)));
				var hint = allHints[i].FindElement(By.XPath(hintXpath + "/p"));
				localHints[i] = new Hint(new LikeButton(likeButton), hint);
			}
			return localHints;
		}

		public string GetHintButtonText()
		{
			return hintsButton.Text;
		}

		public List<Hint> Hints
		{
			get {return GetHints();}
		}

		public bool HasMoreHints()
		{
			return hintsButton.Enabled;
		}

		public void ClickRunButton()
		{
			runSolutionButton.Click();
		}

		public void ClickResetButton()
		{
			resetButton.Click();
		}

		public void ClickHintsButton()
		{
			hintsButton.Click();
		}

		public string GetTextFromSecretCode()
		{
			return driver.FindElement(ElementsId.SecreteCode).Text;
		}

		public string GetUserCodeText()
		{
			return driver.FindElement(ElementsClasses.CodeExercise).Text;
		}

		public ResultType GetRunResult()
		{
			return ResultType.Success;
		}

		private void CheckExerciseSlide()
		{
			CheckCodeMirror();
			CheckButtons();
			CheckHints();
		}

		private void CheckHints()
		{
			return;
		}

		private void CheckButtons()
		{
			if (runSolutionButton == null)
				throw new NotFoundException("run-solution-button отсутствует на странице");
			if (resetButton == null)
				throw new NotFoundException("reset-button отсутствует на странице");
			if (hintsButton == null)
				throw new NotFoundException("get-hints-button отсутствует на странице");
		}

		private void CheckCodeMirror()
		{
			var secretCodeText = driver.FindElement(ElementsId.SecreteCode);
			if (secretCodeText == null)
				throw new NotFoundException("не найдена секретная область кода");
			var codeExerciseField = driver.FindElement(ElementsClasses.CodeExercise);
			if (codeExerciseField == null)
				throw new NotFoundException("не найдена область для кода");
			var codeMirrorObject = driver.FindElement(ElementsClasses.CodeMirror);
			if (codeMirrorObject == null)
				throw new NotFoundException("не найден codemirror");
		}
	}
}
