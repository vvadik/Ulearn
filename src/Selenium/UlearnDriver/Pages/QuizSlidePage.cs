using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using Selenium.UlearnDriver.PageObjects;

namespace Selenium.UlearnDriver.Pages
{
	public class QuizSlidePage : SlidePage
	{
		private readonly List<QuizBlock> blocks;
		private readonly IWebElement submitButton;
		private readonly IWebElement submitAgainButton;
		private readonly IWebElement submitAgainStatus;
		private readonly QuizStatus quizStatus;

		public QuizSlidePage(IWebDriver driver)
			: base(driver)
		{
			submitButton = driver.FindElement(ElementsClasses.QuizSubmitButton);
			submitAgainButton = driver.FindElement(By.XPath(XPaths.QuizSubmitAgainButtonXPath));
			submitAgainStatus = driver.FindElement(By.XPath(XPaths.QuizSubmitAgainStatusXPath));
			quizStatus = GetQuizStatus();
			blocks = FindBlocks();
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
			return blocks;
		}

		private List<QuizBlock> FindBlocks() // пока без FillIn блоков!
		{
			var quizHtmlBlocks = driver.FindElements(By.XPath(XPaths.QuizBlocksXPath)).ToList();
			var quizBlocks = new List<QuizBlock>(quizHtmlBlocks.Count);
			for (var index = 0; index < quizHtmlBlocks.Count; index++)
			{
				var items = quizHtmlBlocks[index].FindElements(By.XPath(XPaths.QuizItemXPath(index)));
				var list = items
					.Select((x, i) => new QuizItem(x, x, quizHtmlBlocks[index]
						.FindElement(By.XPath(XPaths.QuizItemInfoXPath(i))))).ToList();
				quizBlocks[index] = new ChoiseBlock(list,
					UlearnDriver.HasCss(quizHtmlBlocks[index], "checkbox"),
					driver.FindElement(By.XPath(XPaths.QuizQuestionStatusXPath(index))),
					quizStatus);
			}
			return quizBlocks;
			return quizHtmlBlocks //попытка сделать все в LINQ :)
				.Select((y, j) => new ChoiseBlock(y.FindElements(By.XPath(XPaths.QuizItemXPath(j)))
					.Select((x, i) => new QuizItem(x, x, quizHtmlBlocks[j].FindElement(By.XPath(XPaths.QuizItemInfoXPath(i)))))
					.ToList(), 
					UlearnDriver.HasCss(quizHtmlBlocks[j], "checkbox"), 
					driver.FindElement(By.XPath(XPaths.QuizQuestionStatusXPath(j))),
					quizStatus))
				.ToList<QuizBlock>();
		}
	}

	public enum QuizStatus
	{
		Clean,
		NoAttempts,
		HasAttempts
	}
}
