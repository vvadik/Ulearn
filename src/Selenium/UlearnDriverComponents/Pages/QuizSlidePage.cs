using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.PageObjects;

namespace Selenium.UlearnDriverComponents.Pages
{
	public class QuizSlidePage : SlidePage
	{
		private IWebElement submitButton;
		private IWebElement submitAgainButton;
		private IWebElement submitAgainStatus;
		private QuizStatus quizStatus;

		public QuizSlidePage(IWebDriver driver)
			: base(driver)
		{
			Configure();
		}

		private new void Configure()
		{
			base.Configure();
			submitButton = UlearnDriver.FindElementSafely(driver, ElementsClasses.QuizSubmitButton);
			submitAgainButton = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.QuizSubmitAgainButtonXPath));
			submitAgainStatus = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.QuizSubmitAgainStatusXPath));
			quizStatus = GetQuizStatus();
		}

		private QuizStatus GetQuizStatus()
		{
			if (submitButton != null)
				return QuizStatus.Clean;
			if (submitAgainButton != null)
				return QuizStatus.HasAttempts;
			return QuizStatus.NoAttempts;
		}

		public void SubminQuiz()
		{
			if (quizStatus == QuizStatus.Clean)
				submitButton.Click();
			else if (quizStatus == QuizStatus.HasAttempts)
				submitAgainButton.Click();
			else
				throw new Exception("Больше нет попыток!");
		}

		public List<QuizBlock> GetBlocks()
		{
			return FindBlocks();
		}

		private List<QuizBlock> FindBlocks()
		{
			//var quizChoiseHtmlBlocks = driver.FindElements(By.XPath(XPaths.QuizChoiseBlocksXPath)).ToList();
			//var quizBlocks = new List<QuizBlock>(quizChoiseHtmlBlocks.Count);
			//for (var index = 0; index < quizChoiseHtmlBlocks.Count; index++)
			//{
			//	var items = quizChoiseHtmlBlocks[index].FindElements(By.XPath(XPaths.QuizItemXPath(index)));
			//	var list = items
			//		.Select((x, i) => new QuizItem(x, x, quizChoiseHtmlBlocks[index]
			//			.FindElement(By.XPath(XPaths.QuizItemInfoXPath(i))))).ToList();
			//	quizBlocks[index] = new ChoiseBlock(list,
			//		UlearnDriver.HasCss(quizChoiseHtmlBlocks[index], "checkbox"),
			//		driver.FindElement(By.XPath(XPaths.QuizQuestionStatusXPath(index))),
			//		quizStatus);
			//}
			//return quizBlocks;

			//////////////////////////////////////////// вот тут все нормально
			return driver
				.FindElements(By.XPath(XPaths.QuizBlocksXPath))
				.Select(GetBlock)
				.ToList();
			////////////////////////////////////////////

			//return driver.FindElements(By.XPath(XPaths.QuizChoiseBlocksXPath))//попытка сделать все в LINQ :)
			//	.Select((y, j) => new ChoiseBlock(y.FindElements(By.XPath(XPaths.QuizItemXPath(j)))
			//		.Select((x, i) => new QuizItem(x, x, quizChoiseHtmlBlocks[j].FindElement(By.XPath(XPaths.QuizItemInfoXPath(i)))))
			//		.ToList(),
			//		UlearnDriver.HasCss(quizChoiseHtmlBlocks[j], "checkbox"),
			//		driver.FindElement(By.XPath(XPaths.QuizQuestionStatusXPath(j))),
			//		quizStatus))
			//	.Concat<QuizBlock>(//от сюда начинается поиск fillIn блоков, которые не описаны выше не в LINQ (но должно работать LINQ)
			//		driver.FindElements(By.XPath(XPaths.QuizFillInBlocksXPath))
			//			.Select((y, i) => new FillInBlock(y.FindElement(By.XPath(XPaths.QuizFillInBlockField(i))))))
			//	.ToList<QuizBlock>();
		}

		private QuizBlock GetBlock(IWebElement webElement, int index)
		{
			if (UlearnDriver.HasCss(webElement, "quiz-block-input"))
				return new FillInBlock(webElement.FindElement(By.XPath(XPaths.QuizFillInBlockField(index))));
			return new ChoiseBlock(
				webElement
					.FindElements(By.XPath(XPaths.QuizItemXPath(index)))
					.Select((x, i) => new QuizItem(x, x, webElement.FindElement(By.XPath(XPaths.QuizItemInfoXPath(i)))))
					.ToList(),
				UlearnDriver.HasCss(webElement, "checkbox"),
				driver.FindElement(By.XPath(XPaths.QuizQuestionStatusXPath(index))),
				quizStatus);
		}
	}

	public enum QuizStatus
	{
		Clean,
		NoAttempts,
		HasAttempts
	}
}
