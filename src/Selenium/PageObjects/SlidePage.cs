using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.PageObjects
{
	public class SlidePage
	{
		private readonly IWebDriver driver;
		private Rates rates;
		private NavArrows navArrows;
		private readonly TOC TOC;


		public SlidePage(IWebDriver driver, string courseTitle)
		{
			this.driver = driver;
			if (!driver.Title.Equals(courseTitle))
				throw new IllegalLocatorException(string.Format("Это не слайд курса {0}, это: {1}", courseTitle, driver.Title));
			FindSlidePageElements();
			TOC = new TOC(driver, driver.FindElement(By.XPath(XPaths.TOCXPath)), XPaths.TOCXPath);
		}

		private void FindSlidePageElements()
		{
			navArrows = new NavArrows(driver);
			rates = new Rates(driver);
		}

		public TOC GetTOC()
		{
			return TOC;
		}

		public void RateSlide(Rate rate)
		{
			rates.RateSlide(rate);
		}

		public bool IsRateActive(Rate rate)
		{
			return rates.IsActive(rate);
		}

		public SlidePage ClickNextSlide()
		{
			return navArrows.ClickNextButton();
		}

		public SlidePage ClickPrevSlide()
		{
			return navArrows.ClickPrevButton();
		}

		public SolutionsPage ClickSolutionsSlide()
		{
			return navArrows.ClickNextSolutionsButton();
		}
	}

	public class Rates
	{
		private readonly IWebDriver driver;
		private readonly Dictionary<Rate, RateInfo> buttons;
		public Rates(IWebDriver driver)
		{
			this.driver = driver;
			buttons = new Dictionary<Rate, RateInfo>();
			FillButtons();
		}

		private void FillButtons()
		{
			buttons[Rate.NotUnderstand] = new RateInfo(driver.FindElement(By.ClassName(StringValue.GetStringValue(Rate.NotUnderstand))));
			buttons[Rate.Trivial] = new RateInfo(driver.FindElement(By.ClassName(StringValue.GetStringValue(Rate.Trivial))));
			buttons[Rate.Understand] = new RateInfo(driver.FindElement(By.ClassName(StringValue.GetStringValue(Rate.Understand))));
		}

		public void RateSlide(Rate rate)
		{
			buttons[rate].Click();
		}

		public bool IsActive(Rate rate)
		{
			return buttons[rate].isActive;
		}

		class RateInfo
		{
			private readonly IWebElement rateButton;
			public bool isActive;

			public RateInfo(IWebElement button)
			{
				rateButton = button;
				if (button == null)
					throw new NotFoundException("не найдена rate кнопка");
				try
				{
					var cssValue = button.GetCssValue("active");
				}
				catch { }
				isActive = false;
			}

			public void Click()
			{
				rateButton.Click();
				isActive = !isActive;
			}
		}
	}

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
			nextSlideButton.Click();
			return new SlidePage(driver, title);
		}

		public SlidePage ClickPrevButton()
		{
			var title = driver.Title;
			prevSlideButton.Click();
			return new SlidePage(driver, title);
		}

		public SolutionsPage ClickNextSolutionsButton()
		{
			var title = driver.Title;
			nextSolutionsButton.Click();
			return new SolutionsPage(driver, title);
		}
	}
}
