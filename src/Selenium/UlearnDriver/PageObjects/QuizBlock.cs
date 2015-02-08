using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Selenium.UlearnDriver.PageObjects
{
	public abstract class QuizBlock
	{
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

		public ChoiseBlock(List<QuizItem> quizItems, bool isMultiply)
		{
			this.quizItems = quizItems;
			this.isMultiply = isMultiply;
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

	public class QuizItem
	{
		private readonly IWebElement box;
		private readonly IWebElement text;

		public QuizItem(IWebElement box, IWebElement text)
		{
			this.box = box;
			this.text = text;
		}

		public void CheckBox()
		{
			box.Click();
		}

		public string GetText()
		{
			return text.Text;
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
