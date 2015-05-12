using OpenQA.Selenium;

namespace UI.Tests.PageObjects.PageObjects
{
	public abstract class PageObject
	{
		protected IWebDriver Driver;

		protected PageObject(IWebDriver driver)
		{
			Driver = driver;
		}
	}
}
