using System.Collections.Generic;
using OpenQA.Selenium;
using Selenium.UlearnDriver.PageObjects;
using System.Linq;

namespace Selenium.UlearnDriver.Pages
{
	public class SolutionsPage : UlearnContentPage
	{
		public List<SomeoneSolution> Solutions { get; private set; }

		public SolutionsPage(IWebDriver driver)
			: base(driver)
		{
			Solutions = FindSolutions();
		}

		private List<SomeoneSolution> FindSolutions()
		{
			return Enumerable.Range(0, Constants.SomeoneSolutionsCount)
				.Select(x => new SomeoneSolution(driver.FindElement(By.XPath(XPaths.SomeoneSolutionXPath(x))),
							new LikeButton(driver.FindElement(By.XPath(XPaths.SomeoneSolutionLikeXPath(x))))))
				.ToList();
		}
	}
}
