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
		private const string TOCXPath = "/html/body/ul";

		private readonly IWebDriver driver;

		public SlidePage(IWebDriver driver, string courseTitle)
		{
			this.driver = driver;
			if (!driver.Title.Equals(courseTitle))
				throw new IllegalLocatorException(string.Format("Это не слайд курса {0}, это: {1}", courseTitle, driver.Title));
		}

		public TOC GetTOC()
		{
			var TOCElement = driver.FindElement(By.XPath(XPaths.TOCXPath));
			return new TOC(driver, TOCElement, XPaths.TOCXPath);
		}

		public void RateSlide(Rate rate)
		{
			string rateClass = StringValue.GetStringValue(rate);
			var rateButton = driver.FindElement(By.ClassName(rateClass));
			rateButton.Click();
		}

		public void NextSlide()
		{
			var nextButton = driver.FindElement(By.Id("next_slide_button"));
			if (nextButton == null || !nextButton.Enabled)
				return;
			nextButton.Click();
		}

		public void PrevSlide()
		{
			var prevButton = driver.FindElement(By.Id("prev_slide_button"));
			prevButton.Click();
		}
	}
}
