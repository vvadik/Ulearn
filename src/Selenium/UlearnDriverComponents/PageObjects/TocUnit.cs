using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Selenium.UlearnDriverComponents.PageObjects
{
	public class TocUnit
	{
		private readonly IWebElement element;
		private readonly IWebDriver driver;
		private readonly List<SlideControl> slides;
		public TocUnit(IWebDriver driver, IWebElement element, string XPath, int i)
		{
			this.driver = driver;
			this.element = element;
			var index = string.Format("[{0}]", i);
			var newSlideXPath = XPath + index + XPaths.SlideXPath + "/a";
			var newLabelXPath = XPath + index + XPaths.SlideLabelXPath;
			var slidesElements = element
				.FindElements(By.XPath(newSlideXPath))
				.Where(x => x.Text != "Заметки преподавателю" && x.Text != "Статистика и успеваемость")
				.ToList();
			var slidesLabel = element.FindElements(By.XPath(newLabelXPath)).ToList();
			//slides = new Dictionary<string, Lazy<SlideListItem>>();
			slides = new List<SlideControl>(slidesElements.Count);
			//if (slidesElements.Count != slidesLabel.Count)
			//	throw new Exception("incredible mistake on slide!");
			for (var ind = 0; ind < slidesElements.Count; ind++)
			{
				slides.Add(new SlideControl(slidesElements[ind].Text, new SlideListItem(driver, slidesElements[ind], slidesLabel[ind])));
			}
		}

		public UlearnDriver Click()
		{
			element.Click();
			//var TOCElement = driver.FindElement(By.XPath(XPaths.TocXPath));
			return new UlearnDriverComponents.UlearnDriver(driver);
		}

		public IReadOnlyCollection<string> GetSlidesName()
		{
			return slides.Select(x => x.Name).ToList();
		}

		public IReadOnlyCollection<SlideControl> GetSlides()
		{
			return slides;
		}

		//public UlearnDriver ClickOnSlide(string slideName)
		//{
		//	slides[slideName].Value.Click();
		//	//var TOCElement = driver.FindElement(By.XPath(XPaths.TocXPath));
		//	return new UlearnDriverComponents.UlearnDriver(driver);
		//}

		//public SlideLabelInfo GetSlideListItemInfo(string slideName)
		//{
		//	return slides[slideName].Value.GetInfo();
		//}

		
	}

	public class SlideControl
	{
		private readonly SlideListItem item;
		public SlideLabelInfo Item { get { return item.GetInfo(); } }

		public PageType SlideType
		{
			get { return DetermineType(); }
		}

		private PageType DetermineType()
		{
			var info = item.GetInfo();
			return info is ExerciseSlideLabelInfo ? PageType.ExerciseSlidePage :
				info is QuizSlideLabelInfo ? PageType.QuizSlidePage :
					PageType.SlidePage;
		}

		public SlideControl(string name, SlideListItem item)
		{
			Name = name;
			this.item = item;
		}

		public UlearnDriver Click()
		{
			return item.Click();
		}

		public string Name { get; private set; }

	}

	public class SlideListItem
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

		public UlearnDriver Click()
		{
			slideElement.Click();
			return new UlearnDriver(driver);
		}

		public SlideLabelInfo GetInfo()
		{
			if (slideLabelElement == null)
				return new SlideLabelInfo(false, false);

			var isVisited = IsVisited(slideLabelElement);
			var selected = IsSelected(slideElement);

			if (UlearnDriver.HasCss(slideLabelElement, "glyphicon-edit"))
				return new ExerciseSlideLabelInfo(isVisited, selected, IsPassed(slideLabelElement));

			if (UlearnDriver.HasCss(slideLabelElement, "glyphicon-ok"))
				return new SlideLabelInfo(isVisited, selected);

			if (UlearnDriver.HasCss(slideLabelElement, "glyphicon-pushpin"))
			{
				var isPassed = IsPassed(slideLabelElement);
				var isHasAttempts = IsHasAttempts(slideLabelElement);
				return new QuizSlideLabelInfo(isVisited, selected, isPassed, isHasAttempts);
			}

			return new SlideLabelInfo(false, false); //throw new NotFoundException("navbar-label is not found");
		}

		private bool IsSelected(IWebElement element)
		{
			return UlearnDriverComponents.UlearnDriver.HasCss(element, "selected");
		}

		private bool IsHasAttempts(IWebElement webElement)
		{
			if (webElement == null)
				return false;
			return UlearnDriverComponents.UlearnDriver.FindElementSafely(driver, By.Id("quiz-submit-btn")) != null ||
			       UlearnDriverComponents.UlearnDriver.FindElementSafely(driver, By.Id("SubmitQuiz")) != null;
		}

		private static bool IsVisited(IWebElement webElement)
		{
			return UlearnDriverComponents.UlearnDriver.HasCss(webElement, "navbar-label-success") ||
			       UlearnDriverComponents.UlearnDriver.HasCss(webElement, "navbar-label-danger");
		}

		private static bool IsPassed(IWebElement webElement)
		{
			return UlearnDriverComponents.UlearnDriver.HasCss(webElement, "navbar-label-success");
		}
	}

	public class SlideLabelInfo
	{
		private readonly bool isVisited;

		public SlideLabelInfo(bool isVisited, bool selected)
		{
			this.isVisited = isVisited;
			Selected = selected;
		}

		public bool Selected { get; private set; }

		public bool IsVisited()
		{
			return isVisited;
		}
	}

	public class ExerciseSlideLabelInfo : SlideLabelInfo
	{
		private readonly bool isPassed;

		public ExerciseSlideLabelInfo(bool isVisited, bool selected, bool isPassed)
			: base(isVisited, selected)
		{
			this.isPassed = isPassed;
		}

		public bool IsPassed()
		{
			return isPassed;
		}
	}

	public class QuizSlideLabelInfo : SlideLabelInfo
	{
		private readonly bool isPassed;
		private readonly bool isHereAtempts;

		public QuizSlideLabelInfo(bool isVisited, bool selected, bool isPassed, bool isHereAttempts)
			: base(isVisited, selected)
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
