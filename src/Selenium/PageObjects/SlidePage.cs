using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.PageObjects
{
	public class SlidePage
	{
		private IWebDriver driver;

		public SlidePage(IWebDriver driver, string courseTitle)
		{
			this.driver = driver;
			if (!driver.Title.Equals(courseTitle))
				throw new IllegalLocatorException(string.Format("Это не слайд курса {0}, это: {1}", courseTitle, driver.Title));
		}
	}
}
