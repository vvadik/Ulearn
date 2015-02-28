using System;
using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.PageObjects;

namespace Selenium.UlearnDriverComponents.Pages
{
	public class UlearnContentPage : UlearnPage
	{

		private readonly Lazy<NavArrows> navArrows;

		public UlearnContentPage(IWebDriver driver)
			: base(driver)
		{
			navArrows = new Lazy<NavArrows>(() => new NavArrows(driver));
		}
		public UlearnDriverComponents.UlearnDriver ClickNextButton()
		{
			return navArrows.Value.ClickNextButton();
		}

		public UlearnDriverComponents.UlearnDriver ClickPrevButton()
		{
			return navArrows.Value.ClickPrevButton();
		}

		public bool IsActiveNextButton()
		{
			return navArrows.Value.IsActiveNextButton();
		}



		public string GetSlideName()
		{
			var element = UlearnDriverComponents.UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.SlideHeaderXPath));
			return element == null ? null : element.Text;
		}
	}
}
