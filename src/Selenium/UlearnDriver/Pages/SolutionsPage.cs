using System.Collections.Generic;
using OpenQA.Selenium;
using Selenium.UlearnDriver.PageObjects;
using System.Linq;

namespace Selenium.UlearnDriver.Pages
{
	public class SolutionsPage : UlearnContentPage
	{
		public List<SomeoneSolution> solutions { get; protected set; }

		public SolutionsPage(IWebDriver driver)
			: base(driver)
		{
			solutions = FindSolutions();
		}

		private List<SomeoneSolution> FindSolutions()
		{
			return Enumerable.Range(0, Constants.SomeoneSolutionsCount)
				.Select(x => new SomeoneSolution(driver.FindElement(By.XPath(XPaths.SomeoneSolutionXPath(x))),
							new LikeButton(driver.FindElement(By.XPath(XPaths.SomeoneSolutionLikeXPath(x))))))
				.ToList();
		}

		public List<SomeoneSolution> GetSolutions()
		{
			return solutions;
		}
	}
}
