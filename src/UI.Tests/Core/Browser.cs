using System;
using OpenQA.Selenium;

namespace UI.Tests.Core
{
	public class Browser : IGetContext, IDisposable
	{
		private IWebDriver driver;

		public Browser(IWebDriver driver)
		{
			this.driver = driver;
		}

		public TPageObject Get<TPageObject>()
		{
			return driver.Get<TPageObject>();
		}

		public TPageObject[] All<TPageObject>()
		{
			return driver.All<TPageObject>();
		}

		public string WindowTitle { get { return driver.Title; } }

		public TPage Open<TPage>(string url = null)
		{
			driver.Navigate().GoToUrl(url ?? GetPageObjectUrl<TPage>());
			return Get<TPage>();
		}

		private string GetPageObjectUrl<TPageObject>()
		{
			return FindAttr<TPageObject, PageUrlAttribute>().SafeGet(a => a.Url);
		}

		private TAttr FindAttr<T, TAttr>()
		{
			return typeof(T).FindAttr<TAttr>();
		}

		public void Dispose()
		{
			driver.Quit();
			driver.Dispose();
		}
	}
}
