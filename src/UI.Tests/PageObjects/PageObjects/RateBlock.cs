using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace UI.Tests.PageObjects.PageObjects
{
	public class RateBlock : PageObject
	{
		private readonly Dictionary<Rate, RateInfo> buttons;
		public RateBlock(IWebDriver driver) : base(driver)
		{
			buttons = new Dictionary<Rate, RateInfo>();
			FillButtons();
		}

		private void FillButtons()
		{
			buttons[Rate.NotUnderstand] = new RateInfo(Driver.FindElement(By.Id(StringValue.GetStringValue(Rate.NotUnderstand))));
			buttons[Rate.Trivial] = new RateInfo(Driver.FindElement(By.Id(StringValue.GetStringValue(Rate.Trivial))));
			buttons[Rate.Good] = new RateInfo(Driver.FindElement(By.Id(StringValue.GetStringValue(Rate.Good))));
		}

		public void RateSlide(Rate rate)
		{
			buttons[rate].Click();
		}

		public bool IsActive(Rate rate)
		{
			return buttons[rate].isActive;
		}

		public Rate GetCurrentRate()
		{
			return new List<Rate> { Rate.Good, Rate.NotUnderstand, Rate.NotWatched }.FirstOrDefault(IsActive);
		}

		public bool IsRated
		{
			get { return new List<Rate> { Rate.Good, Rate.NotUnderstand, Rate.NotWatched }.Any(IsActive); }
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
