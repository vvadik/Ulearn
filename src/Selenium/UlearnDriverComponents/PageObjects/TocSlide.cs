using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.Interfaces;

namespace Selenium.UlearnDriverComponents.PageObjects
{
	public class TocSlideControl : ITocSlide, IObserver
	{
		private Lazy<SlideListItem> item;
		private readonly IWebDriver driver;
		private readonly int slideIndex;
		private readonly int unitIndex;
		private readonly IObserver parent;
		private Lazy<string> name;
		public SlideLabelInfo Info 
		{
			get
			{
				return item.Value.GetInfo();
			}
		}

		public PageType SlideType
		{
			get { return DetermineType(); }
		}

		private PageType DetermineType()
		{
			var info = item.Value.GetInfo();
			return info is ExerciseSlideLabelInfo ? PageType.ExerciseSlidePage :
				info is QuizSlideLabelInfo ? PageType.QuizSlidePage :
					PageType.SlidePage;
		}

		public TocSlideControl(IWebDriver driver, int slideIndex, int unitIndex, IObserver parent)
		{
			this.driver = driver;
			this.slideIndex = slideIndex;
			this.unitIndex = unitIndex;
			this.parent = parent;
			Configure();
		}

		private void Configure()
		{
			name = new Lazy<string>(
				() => UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.TocSlideXPath(unitIndex, slideIndex))).Text);
			item = new Lazy<SlideListItem>(() =>  new SlideListItem(driver, slideIndex, unitIndex, parent));
		}

		public void Click()
		{
			item.Value.Click();
		}

		public string Name
		{
			get { return name.Value; }
		}

		public void Update()
		{
			Configure();
		}
	}

	public class SlideListItem
	{
		private readonly Lazy<IWebElement> slideElement;
		private readonly Lazy<IWebElement> slideLabelElement;
		private readonly IWebDriver driver;
		private readonly IObserver parent;
		private readonly int slideIndex;
		private readonly int unitIndex;

		public SlideListItem(IWebDriver driver, int slideIndex, int unitIndex, IObserver parent)
		{
			this.slideIndex = slideIndex;
			this.unitIndex = unitIndex;
			this.driver = driver;
			slideElement = new Lazy<IWebElement>(
				() => UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.TocSlideXPath(unitIndex, slideIndex))));
			slideLabelElement = new Lazy<IWebElement>(
				() => UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.TocSlideLabelXPath(unitIndex, slideIndex))));
			this.parent = parent;
		}

		public void Click()
		{
			slideElement.Value.Click();
			parent.Update();
		}

		public SlideLabelInfo GetInfo()
		{
			if (slideLabelElement == null)
				return new SlideLabelInfo(false, false);

			var element = slideLabelElement.Value;
			var isVisited = IsVisited(element);
			var selected = IsSelected(slideElement.Value);

			if (UlearnDriver.HasCss(element, "glyphicon-edit"))
				return new ExerciseSlideLabelInfo(isVisited, selected, IsPassed(element));

			if (UlearnDriver.HasCss(element, "glyphicon-ok"))
				return new SlideLabelInfo(isVisited, selected);

			if (UlearnDriver.HasCss(element, "glyphicon-pushpin"))
			{
				var isPassed = IsPassed(element);
				var isHasAttempts = IsHasAttempts(element);
				return new QuizSlideLabelInfo(isVisited, selected, isPassed, isHasAttempts);
			}

			return new SlideLabelInfo(false, false); //throw new NotFoundException("navbar-label is not found");
		}

		private bool IsSelected(IWebElement element)
		{
			return UlearnDriver.HasCss(element, "selected");
		}

		private bool IsHasAttempts(IWebElement webElement)
		{
			if (webElement == null)
				return false;
			return UlearnDriver.FindElementSafely(driver, By.Id("quiz-submit-btn")) != null ||
				   UlearnDriver.FindElementSafely(driver, By.Id("SubmitQuiz")) != null;
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
			isHereAtempts = isHereAttempts;
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
