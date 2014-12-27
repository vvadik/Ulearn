using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.PageObjects
{
	public class NavArrows
	{
		private readonly IWebDriver driver;
		private readonly IWebElement nextSlideButton;
		private readonly IWebElement prevSlideButton;
		private readonly IWebElement nextSolutionsButton;
		public NavArrows(IWebDriver driver)
		{
			this.driver = driver;
			nextSlideButton = driver.FindElement(ElementsId.NextNavArrow);
			prevSlideButton = driver.FindElement(ElementsId.PrevNavArrow);
			nextSolutionsButton = driver.FindElement(ElementsId.NextSolutionsButton);
			CheckButtons();
		}

		private void CheckButtons()
		{
			if (nextSlideButton == null)
				throw new NotFoundException("не найдена NextSlideButton");
			if (prevSlideButton == null)
				throw new NotFoundException("не найдена PrevSlideButton");
			if (nextSolutionsButton == null)
				throw new NotFoundException("не найдена NextSolutionsButton");
		}

		public SlidePage ClickNextButton()
		{
			var title = driver.Title;
			if (nextSolutionsButton.Enabled)
				nextSlideButton.Click();
			else
				nextSolutionsButton.Click();
			return new SlidePage(driver, title);
		}

		public SlidePage ClickPrevButton()
		{
			var title = driver.Title;
			prevSlideButton.Click();
			return new SlidePage(driver, title);
		}

		//SolutionsPage ClickNextSolutionsButton()
		//{
		//	var title = driver.Title;
		//	nextSolutionsButton.Click();
		//	return new SolutionsPage(driver, title);
		//}

		public bool IsActiveNextButton()
		{
			return IsActive(nextSlideButton.Displayed ? nextSlideButton : nextSolutionsButton);
		}


		//public bool IsActiveNextSolutionButton()
		//{
		//	return IsActive(nextSolutionsButton);
		//}

		private static bool IsActive(IWebElement button)
		{
			try
			{
				button.GetCssValue("block_next");
				return true;
			}
			catch 
			{
				return false;
			}
		}

	}
}
