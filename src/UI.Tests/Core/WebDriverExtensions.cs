using OpenQA.Selenium;

namespace UI.Tests.Core
{
	public static class WebDriverExtensions
	{
		public static IWebElement RootElement(this IWebDriver driver)
		{
			return driver.FindElement(By.TagName("html"));
		}
	}
}