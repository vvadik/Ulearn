using System;
using OpenQA.Selenium;
using Selenium.UlearnDriver.PageObjects;
using Rates = Selenium.UlearnDriver.PageObjects.Rates;

namespace Selenium.UlearnDriver.Pages
{
	public class SlidePage : UlearnContentPage
	{
		private Lazy<Rates> rates;


		public SlidePage(IWebDriver driver)
			: base(driver)
		{
			FindSlidePageElements();
		}

		private void FindSlidePageElements()
		{
			rates = new Lazy<Rates>(() => new Rates(driver));
		}

		public void RateSlide(Rate rate)
		{
			rates.Value.RateSlide(rate);
		}

		public bool IsRateActive(Rate rate)
		{
			return rates.Value.IsActive(rate);
		}
	}
}
