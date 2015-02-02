using System;
using OpenQA.Selenium;
using Selenium.UlearnDriver.PageObjects;
using Rates = Selenium.UlearnDriver.PageObjects.Rates;

namespace Selenium.UlearnDriver.Pages
{
	public class SlidePage : UlearnPage
	{
		private readonly IWebDriver driver;
		private Lazy<Rates> rates;
		private Lazy<NavArrows> navArrows;
		//private readonly Lazy<TOC> TOC;


		public SlidePage(IWebDriver driver)
			: base(driver)
		{
			this.driver = driver;
			FindSlidePageElements();
			//TOC = new Lazy<TOC>(() => new TOC(driver, driver.FindElement(By.XPath(XPaths.TOCXPath)), XPaths.TOCXPath));
		}

		public SlidePage(IWebDriver driver, string courseTitle)
			: base(driver)
		{
			this.driver = driver;
			if (!driver.Title.Equals(courseTitle))
				throw new IllegalLocatorException(string.Format("Это не слайд курса {0}, это: {1}", courseTitle, driver.Title));
			FindSlidePageElements();
			//TOC = new Lazy<TOC>(() => new TOC(driver, driver.FindElement(By.XPath(XPaths.TOCXPath)), XPaths.TOCXPath));
		}

		private void FindSlidePageElements()
		{
			navArrows = new Lazy<NavArrows>(() => new NavArrows(driver));
			rates = new Lazy<Rates>(() => new Rates(driver));
		}

		//public TOC GetTOC()
		//{
		//	return TOC.Value;
		//}

		public void RateSlide(Rate rate)
		{
			rates.Value.RateSlide(rate);
		}

		public bool IsRateActive(Rate rate)
		{
			return rates.Value.IsActive(rate);
		}

		public UlearnDriver ClickNextButton()
		{
			return navArrows.Value.ClickNextButton();
		}

		public UlearnDriver ClickPrevButton()
		{
			return navArrows.Value.ClickPrevButton();
		}

		//public SolutionsPage ClickSolutionsSlide()
		//{
		//	return navArrows.Value.ClickNextSolutionsButton();
		//}

		public bool IsActiveNextButton()
		{
			return navArrows.Value.IsActiveNextButton();
		}

		//public bool IsActiveNextSolutionsButton()
		//{
		//	return navArrows.Value.IsActiveNextSolutionButton();
		//}
	}
}
