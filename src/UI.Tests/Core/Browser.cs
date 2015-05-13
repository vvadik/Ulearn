using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace UI.Tests.Core
{
	public class Browser : IGetContext, IDisposable
	{
		public static Browser CreateDefault()
		{
			return new Browser(new ChromeDriver(), new Screenshoter(new DirectoryInfo("../../screenshots")), "https://localhost:44300/");
		}

		public IWebDriver Driver { get; private set; }
		private Screenshoter screenshoter;
		private readonly string baseUrl;

		public Browser(IWebDriver driver, Screenshoter screenshoter, string baseUrl)
		{
			Driver = driver;
			this.screenshoter = screenshoter;
			this.baseUrl = baseUrl;
		}

		public string WindowTitle { get { return Driver.Title; } }

		public void SaveScreenshot()
		{
			var filename = screenshoter.SaveScreenshot(Driver);
			Console.WriteLine("Screenshot saved to: {0}", filename);
		}

		public TPageObject Get<TPageObject>()
		{
			return Get<TPageObject>(null);
		}

		public TPageObject[] All<TPageObject>()
		{
			return All<TPageObject>(null);
		}

		public TPageObject Get<TPageObject>(ISearchContext parent)
		{
			var context = parent ?? Driver;
			return Safe(() => context.Get<TPageObject>(this),
				"Get " + typeof(TPageObject));
		}

		public TPageObject[] All<TPageObject>(ISearchContext parent)
		{
			var context = parent ?? Driver;
			return Safe(() => context.All<TPageObject>(this),
				"Get all " + typeof(TPageObject));
		}

		public TPage Open<TPage>(string url = null)
		{
			url = url ?? GetPageObjectUrl<TPage>();
			Safe(() => Driver.Navigate().GoToUrl(baseUrl + url), "Open " + url); //TODO absolute urls support
			return Get<TPage>();
		}

		public TResult Safe<TResult>(Func<TResult> action, string actionDescription = null)
		{
			try
			{
				return action();
			}
			catch (Exception)
			{
				HandleException(actionDescription);
				throw;
			}
		}

		public void Safe(Action action, string actionDescription = null)
		{
			try
			{
				action();
			}
			catch (Exception)
			{
				HandleException(actionDescription);
				throw;
			}
		}

		private void HandleException(string actionDescription)
		{
			if (actionDescription != null)
				Console.WriteLine("Action: " + actionDescription);
			SaveScreenshot();
			Console.WriteLine("Window title: " + WindowTitle);
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
			Driver.Quit();
			Driver.Dispose();
		}
	}
}
