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
				slides[slidesElements[ind].Text] = new Lazy<SlideListItem>(() => new SlideListItem(driver, slidesElements[lazyInd], slidesLabel[lazyInd]));
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

		public SlideLabelInfo GetSlideListItemInfo(string slideName)
		{
			return slides[slideName].Value.GetInfo();
		}

		class SlideListItem
		{
			private readonly IWebElement slideElement;
			private readonly IWebElement slideLabelElement;
			private readonly IWebDriver driver;

			public SlideListItem(IWebDriver driver, IWebElement slideElement, IWebElement slideLabelElement)
			{
				this.driver = driver;
				this.slideElement = slideElement;
				this.slideLabelElement = slideLabelElement;
			}

			public void Click()
			{
				slideElement.Click();
			}

			public SlideLabelInfo GetInfo()
			{
				if (slideLabelElement == null)
					return new SlideLabelInfo(false);

				var isVisited = IsVisited(slideLabelElement);

				if (UlearnDriver.HasCss(slideLabelElement, "glyphicon-edit"))
					return new ExerciseSlideLabelInfo(isVisited, IsPassed(slideLabelElement));

				if (UlearnDriver.HasCss(slideLabelElement, "glyphicon-ok"))
					return new SlideLabelInfo(isVisited);

				if (UlearnDriver.HasCss(slideLabelElement, "glyphicon-pushpin"))
				{
					var isPassed = IsPassed(slideLabelElement);
					var isHasAttempts = IsHasAttempts(slideLabelElement);
					return new TestSlideLabelInfo(isVisited, isPassed, isHasAttempts);
				}

				throw new NotFoundException("navbar-label is not found");
			}

			private bool IsHasAttempts(IWebElement webElement)
			{
				return driver.FindElement(By.Id("quiz-submit-btn")) != null ||
					   driver.FindElement(By.Id("SubmitQuiz")) != null;
			}

			private static bool IsVisited(IWebElement webElement)
			{
				return UlearnDriver.HasCss(webElement, "navbar-label-success") ||
					   UlearnDriver.HasCss(webElement, "navbar-label-danger");
			}

			private static bool IsPassed(IWebElement webElement)
			{
				return UlearnDriver.HasCss(webElement, "navbar-label-success");
			}
		}
	}

	public class SlideLabelInfo
	{
		private readonly bool isVisited;

		public SlideLabelInfo(bool isVisited)
		{
			this.isVisited = isVisited;
		}

		public bool IsVisited()
		{
			return isVisited;
		}
	}

	public class ExerciseSlideLabelInfo : SlideLabelInfo
	{
		private readonly bool isPassed;

		public ExerciseSlideLabelInfo(bool isVisited, bool isPassed)
			: base(isVisited)
		{
			this.isPassed = isPassed;
		}

		public bool IsPassed()
		{
			return isPassed;
		}
	}

	public class TestSlideLabelInfo : SlideLabelInfo
	{
		private readonly bool isPassed;
		private readonly bool isHereAtempts;

		public TestSlideLabelInfo(bool isVisited, bool isPassed, bool isHereAttempts)
			: base(isVisited)
		{
			this.isPassed = isPassed;
			this.isHereAtempts = isHereAttempts;
		}

		public bool IsPassed()//возможно, логика будет отличаться от ExerciseSlideLabelInfo, потом посмотрим.
		{
			return isPassed;
		}

		public bool IsHereAttempts()
		{
			return isHereAtempts;
		}
	}
}
