using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.Interfaces;

namespace Selenium.UlearnDriverComponents.PageObjects
{
	public class TocUnit : ITocUnit
	{
		private readonly IWebDriver driver;
		private List<Lazy<ITocSlide>> slides;
		private readonly int unitIndex;
		private IWebElement headerElement;

		public TocUnit(IWebDriver driver, int unitIndex)
		{
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
				slides.Add(new Lazy<ITocSlide>(() => new TocSlideControl(driver, lazyIndex, unitIndex)));
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
		}

		public IReadOnlyCollection<string> GetSlidesName()
		{
			return slides.Select(x => x.Value.Name).ToList();
		}

		public IReadOnlyCollection<ITocSlide> GetSlides()
		{
			return slides.Select(x => x.Value).ToList();
		}

		public bool Collapse
		{
			get { return IsCollapse(); }
		}

		private bool IsCollapse()
		{
			var element = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.TocUnitHeaderCollapseInfoXPath(unitIndex)));
			return UlearnDriver.HasCss(element, "in");
		}
	}
}
