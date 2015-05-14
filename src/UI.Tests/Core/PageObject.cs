using System;
using OpenQA.Selenium;

namespace UI.Tests.Core
{
	public class PageObject : IGetContext
	{
		[UsedImplicitly]
		public Browser Browser;
		
		[UsedImplicitly]
		[CanBeNull]
		public IWebElement Element;

		public TPageObject Get<TPageObject>()
		{
			return Browser.Get<TPageObject>(Element);
		}

		public TPageObject[] All<TPageObject>()
		{
			return Browser.All<TPageObject>(Element);
		}

		public TPageObect ClickAndOpen<TPageObect>()
		{
			if (Element == null)
				throw new Exception("Cant click on page object without specified Element");
			Browser.Safe(Element.Click, "Mouse click " + Element.Text);
			return Browser.Get<TPageObect>();
		}
	}
}