using System;
using OpenQA.Selenium;

namespace UI.Tests.Core
{
	public class PageObject : IGetContext
	{
		[UsedImplicitly]
		protected Browser browser;
		
		[UsedImplicitly]
		[CanBeNull]
		protected IWebElement element;

		public TPageObject Get<TPageObject>()
		{
			return browser.Get<TPageObject>(element);
		}

		public TPageObject[] All<TPageObject>()
		{
			return browser.All<TPageObject>(element);
		}

		public TPageObect ClickAndOpen<TPageObect>()
		{
			if (element == null)
				throw new Exception("Cant click on page object without specified element");
			browser.Safe(element.Click, "Mouse click " + element.Text);
			return browser.Get<TPageObect>();
		}
	}
}