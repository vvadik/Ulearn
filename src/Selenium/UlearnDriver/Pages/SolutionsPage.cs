using OpenQA.Selenium;

namespace Selenium.UlearnDriver.Pages
{
	public class SolutionsPage : UlearnPage
	{
		//private IWebDriver driver;

		public SolutionsPage(IWebDriver driver)
			: base(driver)
		{ }

		public SolutionsPage(IWebDriver driver, string courseTitle)
			: base(driver)
		{
			//this.driver = driver;
			if (!driver.Title.Equals(courseTitle))
				throw new IllegalLocatorException(string.Format("Это не слайд курса {0}, это: {1}", courseTitle, driver.Title));
		}
	}
}
