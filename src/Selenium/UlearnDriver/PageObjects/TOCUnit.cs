using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Selenium.UlearnDriver.PageObjects
{
	public class TOCUnit
	{
		private readonly IWebElement element;
		private readonly IWebDriver driver;
		private readonly Dictionary<string, Lazy<SlideListItem>> slides;
		public TOCUnit(IWebDriver driver, IWebElement element, string XPath, int i)
		{
			this.driver = driver;
			this.element = element;
			var index = string.Format("[{0}]", i);
			var newSlideXPath = XPath + index + XPaths.SlideXPath;
			var newLabelXPath = XPath + index + XPaths.SlideLabelXPath;
			var slidesElements = element.FindElements(By.XPath(newSlideXPath)).ToList();
			var slidesLabel = element.FindElements(By.XPath(newLabelXPath)).ToList();
			slides = new Dictionary<string, Lazy<SlideListItem>>();
			if (slidesElements.Count != slidesLabel.Count)
				throw new Exception("incredible mistake in slide!");
			for (var ind = 0; ind < slidesElements.Count; ind++)
			{
				var lazyInd = ind;
				slides[slidesElements[ind].Text] = new Lazy<SlideListItem>(() => new SlideListItem(slidesElements[lazyInd], slidesLabel[lazyInd]));
			}
		}

		public UlearnDriver Click()
		{
			element.Click();
			var TOCElement = driver.FindElement(By.XPath(XPaths.TOCXPath));
			return new UlearnDriver(driver);
		}

		public string[] GetSlidesNames()
		{
			return slides.Keys.ToArray();
		}

		public UlearnDriver ClickOnSlide(string slideName)
		{
			slides[slideName].Value.Click();
			var TOCElement = driver.FindElement(By.XPath(XPaths.TOCXPath));
			return new UlearnDriver(driver);
		}

		public SlideListItemInfo GetSlideListItemInfo(string slideName)
		{
			return slides[slideName].Value.GetInfo();
		}

		class SlideListItem
		{
			private readonly IWebElement slideElement;
			private readonly IWebElement slideLabelElement;

			public SlideListItem(IWebElement slideElement, IWebElement slideLabelElement)
			{
				this.slideElement = slideElement;
				this.slideLabelElement = slideLabelElement;
			}

			public void Click()
			{
				slideElement.Click();
			}

			public SlideListItemInfo GetInfo()
			{
				ListItemType itemType;
				throw new NotImplementedException();
				//if (slideLabelElement.GetCssValue(""))
			}
		}
	}

	public enum ListItemType
	{
		Exersice,
		Theory,
		Test
	}

	public class SlideListItemInfo
	{
		private readonly ListItemType type;
		private readonly bool isVisited;
		private readonly bool isPassed;

		public SlideListItemInfo(ListItemType type, bool isVisited, bool isPassed)
		{
			this.type = type;
			this.isVisited = isVisited;
			this.isPassed = isPassed;
		}

		public ListItemType GetListItemType()
		{
			return type;
		}

		public bool IsVisited()
		{
			return isVisited;
		}

		public bool IsPassed()
		{
			return isPassed;
		}
	}
}
