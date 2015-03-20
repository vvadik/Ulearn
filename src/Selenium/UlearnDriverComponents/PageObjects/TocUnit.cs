using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.Interfaces;

namespace Selenium.UlearnDriverComponents.PageObjects
{
	public class TocUnit : ITocUnit, IObserver
	{
		private readonly IWebDriver driver;
		private List<Lazy<ITocSlide>> slides;
		private readonly IObserver parent;
		private readonly int unitIndex;
		private IWebElement headerElement;
		private readonly HashSet<IObserver> observers = new HashSet<IObserver>();

		public TocUnit(IWebDriver driver, int unitIndex, IObserver parent)
		{
			this.parent = parent;
			this.driver = driver;
			this.unitIndex = unitIndex;

			Configure();
		}

		private void Configure()
		{
			headerElement = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.TocUnitHeaderXPath(unitIndex)));

			//var courseTitle = UlearnDriver.ConvertCourseTitle(driver.Title);
			//var unitName = headerElement.Text;
			var slidesCount = UlearnDriver.FindElementsSafely(driver, By.XPath(XPaths.TocSlidesXPath(unitIndex))).Count; //UlearnDriver.courseManager.GetCourse(courseTitle).Slides.Count(x => x.Info.UnitName == unitName);
			slides = new List<Lazy<ITocSlide>>(slidesCount);
			for (var ind = 0; ind < slidesCount; ind++)
			{
				var lazyIndex = ind;
				slides.Add(new Lazy<ITocSlide>(() => new TocSlideControl(driver, lazyIndex, unitIndex, parent)));
				//if (slides.Count > ind)
				//{
				//	if (slides[ind] != null)
				//		continue;
				//	slides[ind] = new TocSlideControl(driver, ind, unitIndex, parent);
				//	//observers.Add(slides[ind] as IObserver);
				//}
				//else
				//{
				//	slides.Add(new TocSlideControl(driver, ind, unitIndex, parent));
				//	//observers.Add(slides[ind] as IObserver);
				//}
			}
		}

		public void Click()
		{
			headerElement.Click();
			parent.Update();
		}

		public IReadOnlyCollection<string> GetSlidesName()
		{
			return slides.Select(x => x.Value.Name).ToList();
		}

		public IReadOnlyCollection<ITocSlide> GetSlides()
		{
			return slides.Select(x => x.Value).ToList();
		}

		public void Update()
		{
			Configure();
			//foreach (var o in observers)
			//	o.Update();
		}
	}
}
