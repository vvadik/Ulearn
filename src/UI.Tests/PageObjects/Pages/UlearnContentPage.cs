using OpenQA.Selenium;
using UI.Tests.PageObjects.PageObjects;

namespace UI.Tests.PageObjects.Pages
{
	public class UlearnContentPage : UlearnPage
	{
		public UlearnContentPage(IWebDriver driver)
			: base(driver)
		{
			Configure();
		}

		protected void Configure()
		{
		}

		public NavigationBlock GetNavArrows()
		{
			return new NavigationBlock(driver);
		}

		public string GetSlideName()
		{
			var element = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.SlideHeaderXPath));
			return element == null ? null : element.Text;
		}
	}
}
