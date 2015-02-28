using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.Pages;

namespace Selenium.UlearnDriverComponents.PageObjects
{
	public abstract class QuizBlock
	{
	}

	public class AbstractQuestionBlock : QuizBlock
	{
		private readonly int blockIndex;

		public AbstractQuestionBlock(int blockIndex)
		{
			this.blockIndex = blockIndex;
		}

		public int GetBlockIndex()
		{
			return blockIndex;
		}
	}

	public class TextBlock : QuizBlock
	{
		private readonly string text;

		public TextBlock(string text)
		{
			this.text = text;
		}

		public string GetText()
		{
			return text;
		}
	}

	public class FillInBlock : QuizBlock
	{
		private readonly IWebElement field;

		public FillInBlock(IWebElement filed)
		{
			this.field = filed;
		}

		public void FillFiled(string text)
		{
			field.SendKeys(text);
		}
	}

	public class ChoiseBlock : QuizBlock
	{
		private readonly List<QuizItem> quizItems;
		private readonly bool isMultiply;
		private readonly Status blockStatus;

		public ChoiseBlock(List<QuizItem> quizItems, bool isMultiply, IWebElement statusElement, QuizStatus quizStatus)
		{
			this.quizItems = quizItems;
			this.isMultiply = isMultiply;
			blockStatus = quizItems.Any(x => x.GetStatus() == Status.Wrong) ? Status.Wrong :
				quizItems.Any(x => x.GetStatus() == Status.Right) ? Status.Right : Status.Undefined;
			var localStatus = UlearnDriverComponents.UlearnDriver.HasCss(statusElement, "glyphicon-ok") ? Status.Right :
				UlearnDriverComponents.UlearnDriver.HasCss(statusElement, "glyphicon-remove") ? Status.Wrong :
					Status.Undefined;
			if (localStatus != blockStatus)
				throw new Exception("Не верно произведена оценка квиза");
			if (quizStatus == QuizStatus.NoAttempts && localStatus == Status.Undefined)
				throw new Exception("Не произведена проверка квиза");
			if (quizStatus == QuizStatus.HasAttempts && quizItems.Any(x => x.GetStatus() == Status.Wrong || x.GetStatus() == Status.Right))
				throw new Exception("Отмечены верные/неверные варианты ответа при имеющихся попытках");
			if (quizStatus == QuizStatus.Clean && (quizItems.Any(x => x.GetStatus() == Status.Wrong || x.GetStatus() == Status.Right)) || (localStatus != Status.Undefined))
				throw new Exception("Отмечены верные/неверные варианты ответа или выставлены оценки квиз-блокам, когда квиз еще не решался");
		}

		public Status GetBlockStatus()
		{
			return blockStatus;
		}

		public List<String> GetQuizItems()
		{
			return quizItems.Select(x => x.GetText()).ToList();
		}

		public void ChoiseElement(string element)
		{
			var quizItem = quizItems.FirstOrDefault(x => x.GetText() == element);
			if (quizItem == null)
				throw new NotFoundException(string.Format("item with name {0} not found", element));
			quizItem.CheckBox();
		}

		public bool IsMultiply()
		{
			return isMultiply;
		}
	}

	public enum Status
	{
		Right,
		Wrong,
		Undefined
	}
	public class QuizItem
	{
		private readonly IWebElement box;
		private readonly IWebElement textElement;
		private readonly IWebElement infoElement;
		private readonly Status itemStatus;

		public QuizItem(IWebElement box, IWebElement textElement, IWebElement infoElement)
		{
			this.box = box;
			this.textElement = textElement;
			this.infoElement = infoElement;
			itemStatus = UlearnDriverComponents.UlearnDriver.HasCss(infoElement, "wrong-quiz") ? Status.Wrong : 
				UlearnDriverComponents.UlearnDriver.HasCss(infoElement, "right-quiz") ? Status.Right : Status.Undefined;
		}

		public void CheckBox()
		{
			box.Click();
		}

		public Status GetStatus()
		{
			return itemStatus;
		}

		public string GetText()
		{
			return textElement.Text;
		}
	}

	public class IsTrueBlock : QuizBlock
	{
		private readonly IWebElement falseBox;
		private readonly IWebElement trueBox;

		public IsTrueBlock(IWebElement trueBox, IWebElement falseBox)
		{
			this.trueBox = trueBox;
			this.falseBox = falseBox;
		}

		public void Choose(bool answer)
		{
			if (answer)
				trueBox.Click();
			else
				falseBox.Click();
		}
	}
}
