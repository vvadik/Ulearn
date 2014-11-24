using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.PageObjects
{
	public class TOCUnit
	{
		private readonly IWebElement element;
		private const string slideXPath = "/ul/li";
		private const string slideLabelXPath = "/ul/i";
		private IWebDriver driver;
		private Dictionary<string, SlideListItem> slides;
		public TOCUnit(IWebDriver driver, IWebElement element, string XPath, int i)
		{
			this.driver = driver;
			this.element = element;
			var index = string.Format("[{0}]", i);
			var newSlideXPath = XPath + index + slideXPath;
			var newLabelXPath = XPath + index + slideLabelXPath;
			var slidesElements = element.FindElements(By.XPath(newSlideXPath)).ToList();
			var slidesLabel = element.FindElements(By.XPath(newLabelXPath)).ToList();
			slides = new Dictionary<string, SlideListItem>();
			if (slidesElements.Count != slidesLabel.Count)
				throw new Exception("incredible mistake in slide!");
			for (var ind = 0; ind < slidesElements.Count; ind++)
				slides[slidesElements[ind].Text] = new SlideListItem(slidesElements[ind], slidesLabel[ind]);
		}

		public TOC Click()
		{
			element.Click();
			var TOCElement = driver.FindElement(By.XPath(XPaths.TOCXPath));
			return new TOC(driver, TOCElement, XPaths.TOCXPath);
		}

		public string[] GetSlidesNames()
		{
			return slides.Keys.ToArray();
		}

		public TOC ClickOnSlide(string slideName)
		{
			slides[slideName].Click();
			var TOCElement = driver.FindElement(By.XPath(XPaths.TOCXPath));
			return new TOC(driver, TOCElement, XPaths.TOCXPath);
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
		}
	}
}
