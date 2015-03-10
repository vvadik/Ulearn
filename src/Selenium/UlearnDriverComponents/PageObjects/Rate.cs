using System.Collections.Generic;
using OpenQA.Selenium;

namespace Selenium.UlearnDriverComponents.PageObjects
{
	public class Rates
	{
		private readonly IWebDriver driver;
		private readonly Dictionary<Rate, RateInfo> buttons;
		public Rates(IWebDriver driver)
		{
			this.driver = driver;
			buttons = new Dictionary<Rate, RateInfo>();
			FillButtons();
		}

		private void FillButtons()
		{
			buttons[Rate.NotUnderstand] = new RateInfo(driver.FindElement(By.ClassName(StringValue.GetStringValue(Rate.NotUnderstand))));
			buttons[Rate.Trivial] = new RateInfo(driver.FindElement(By.ClassName(StringValue.GetStringValue(Rate.Trivial))));
			buttons[Rate.Good] = new RateInfo(driver.FindElement(By.ClassName(StringValue.GetStringValue(Rate.Good))));
		}

		public void RateSlide(Rate rate)
		{
			buttons[rate].Click();
		}

		public bool IsActive(Rate rate)
		{
			return buttons[rate].isActive;
		}

		class RateInfo
		{
			private readonly IWebElement rateButton;
			public bool isActive;

			public RateInfo(IWebElement button)
			{
				rateButton = button;
				if (button == null)
					throw new NotFoundException("не найдена rate кнопка");
				try
				{
					button.GetCssValue("active");
					isActive = true;
				}
				catch
				{
					isActive = false;
				}
			}

			public void Click()
			{
				rateButton.Click();
				isActive = !isActive;
			}
		}
	}
}
