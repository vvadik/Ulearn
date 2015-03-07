using System;
using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.PageObjects;

namespace Selenium.UlearnDriverComponents.Pages
{
	public class UlearnContentPage : UlearnPage, IObserver
	{
		private Lazy<NavArrows> navArrows;

		public UlearnContentPage(IWebDriver driver, IObserver parent)
			: base(driver, parent)
		{
			Configure();
		}

		protected void Configure()
		{
			navArrows = new Lazy<NavArrows>(() => new NavArrows(driver));
		}

		public UlearnDriver ClickNextButton()
		{
			parent.Update();
			return navArrows.Value.ClickNextButton();
		}

		public UlearnDriver ClickPrevButton()
		{
			parent.Update();
			return navArrows.Value.ClickPrevButton();
		}

		public bool IsActiveNextButton()
		{
			return navArrows.Value.IsActiveNextButton();
		}

		public string GetSlideName()
		{
			var element = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.SlideHeaderXPath));
			return element == null ? null : element.Text;
		}

		public new void Update()
		{
			Configure();
		}
	}
}
