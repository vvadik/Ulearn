using System;
using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.Interfaces;
using Selenium.UlearnDriverComponents.PageObjects;

namespace Selenium.UlearnDriverComponents.Pages
{
	public class UlearnContentPage : UlearnPage
	{
		private Lazy<NavigationBlock> navArrows;

		public UlearnContentPage(IWebDriver driver)
			: base(driver)
		{
			Configure();
		}

		protected void Configure()
		{
			navArrows = new Lazy<NavigationBlock>(() => new NavigationBlock(driver));
		}

		public NavigationBlock GetNavArrows()
		{
			return navArrows.Value;
		}

		public string GetSlideName()
		{
			var element = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.SlideHeaderXPath));
			return element == null ? null : element.Text;
		}
	}
}
