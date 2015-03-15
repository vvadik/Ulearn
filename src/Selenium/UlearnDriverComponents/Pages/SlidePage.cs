using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.Interfaces;
using Selenium.UlearnDriverComponents.PageObjects;

namespace Selenium.UlearnDriverComponents.Pages
{
	public class SlidePage : UlearnContentPage
	{
		private Lazy<Rates> rates;
		private List<SlidePageBlock> blocks;
		private IWebElement groupSelector;
		private IWebElement groupSelectButton;

		public bool IsUserFirstVisit { get; private set; }


		public SlidePage(IWebDriver driver, IObserver parent)
			: base(driver, parent)
		{
			Configure();
		}

		private new void Configure()
		{
			base.Configure();
			rates = new Lazy<Rates>(() => new Rates(driver));
			var modal = UlearnDriver.FindElementSafely(driver, By.Id("selectGroupModal"));
			if (modal != null)
			{
				IsUserFirstVisit = true;
				groupSelector = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.UserGroupSelectField));
				groupSelectButton = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.UserGroupSelectButton));
			}
			var blockElements = UlearnDriver.FindElementsSafely(driver, By.XPath(XPaths.SeleniumTextBlockXPath));
			blocks = UnionSubBlucks(blockElements.Where(
				x => x.TagName == "textarea" ||
				x.TagName == "p" ||
				(x.TagName == "div" && UlearnDriver.HasCss(x, "video-container")))
				.Select(CreateBlock));
		}

		private List<SlidePageBlock> UnionSubBlucks(IEnumerable<SlidePageBlock> subBlocks)
		{
			return subBlocks.ToList();//TODO
		}

		public void SelectGroup(string groupName)
		{
			groupSelector.SendKeys(groupName);
			groupSelectButton.Click();
			parent.Update();
		}

		private static SlidePageBlock CreateBlock(IWebElement element)
		{
			if (element.TagName == "p")
				return new SlidePageTextBlock(element.Text);
			if (UlearnDriver.HasCss(element, "video-container"))
				return new SlidePageVideoBlock();
			if (UlearnDriver.HasCss(element, "code-sample"))
				return new SlidePageCodeBlock(element.Text, false);
			return new SlidePageCodeBlock(element.Text, true);
		}

		public IReadOnlyCollection<SlidePageBlock> Blocks { get { return blocks; } }

		public void RateSlide(Rate rate)
		{
			rates.Value.RateSlide(rate);
			//parent.Update();
		}

		public bool IsSlideRated()
		{
			return IsActiveNextButton();
		}

		public Rate GetCurrentRate()
		{
			return new List<Rate> { Rate.Good, Rate.NotUnderstand, Rate.NotWatched }.FirstOrDefault(IsRateActive);
		}

		public bool IsRateActive(Rate rate)
		{
			return rates.Value.IsActive(rate);
		}
	}
}
