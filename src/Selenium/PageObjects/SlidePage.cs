using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using uLearn;
using uLearn.Web;

namespace Selenium.PageObjects
{
	public class SlidePage
	{
		private readonly IWebDriver driver;
		private Lazy<Rates> rates;
		private Lazy<NavArrows> navArrows;
		private readonly Lazy<TOC> TOC;


		public SlidePage(IWebDriver driver, string courseTitle)
		{
			this.driver = driver;
			if (!driver.Title.Equals(courseTitle))
				throw new IllegalLocatorException(string.Format("Это не слайд курса {0}, это: {1}", courseTitle, driver.Title));
			FindSlidePageElements();
			TOC = new Lazy<TOC>(() => new TOC(driver, driver.FindElement(By.XPath(XPaths.TOCXPath)), XPaths.TOCXPath));
		}

		private void FindSlidePageElements()
		{
			navArrows = new Lazy<NavArrows>(() => new NavArrows(driver));
			rates = new Lazy<Rates>(() => new Rates(driver));
		}

		public TOC GetTOC()
		{
			return TOC.Value;
		}

		public void RateSlide(Rate rate)
		{
			rates.Value.RateSlide(rate);
		}

		public bool IsRateActive(Rate rate)
		{
			return rates.Value.IsActive(rate);
		}

		public SlidePage ClickNextSlide()
		{
			return navArrows.Value.ClickNextButton();
		}

		public SlidePage ClickPrevSlide()
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
