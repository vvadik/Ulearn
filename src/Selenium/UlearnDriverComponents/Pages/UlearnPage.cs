using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.Interfaces;

namespace Selenium.UlearnDriverComponents.Pages
{
	public class UlearnPage
	{
		protected readonly IWebDriver driver;

		public UlearnPage(IWebDriver driver)
		{
			this.driver = driver;
		}

		public string GetTitle()
		{
			return driver.Title;
		}

		public PageType GetPageType()
		{
			var title = GetTitle();
			if (title == Titles.RegistrationPageTitle)
				return PageType.RegistrationPage;
			if (title == Titles.StartPageTitle)
				return PageType.StartPage;
			if (title == Titles.SignInPageTitle)
				return PageType.SignInPage;
			if (UlearnDriver.FindElementSafely(driver, By.ClassName("side-bar")) == null)
				return PageType.IncomprehensibleType;
			var element = UlearnDriver.FindElementSafely(driver, By.ClassName("page-header"));
			if (element != null && element.Text == "Решения")
				return PageType.SolutionsPage;
			if (UlearnDriver.FindElementSafely(driver, ElementsClasses.RunSolutionButton) != null)
				return PageType.ExerciseSlidePage;
			if (UlearnDriver.FindElementSafely(driver, By.ClassName("quiz")) != null)
				return PageType.QuizSlidePage;
			return PageType.SlidePage;
		}

		private static readonly Dictionary<PageType, Func<IWebDriver, UlearnPage>> PageFabric =
			new Dictionary<PageType, Func<IWebDriver, UlearnPage>>
		{
			{PageType.SignInPage, driver => new SignInPage(driver) },
			{PageType.SlidePage, driver => new SlidePage(driver) },
			{PageType.ExerciseSlidePage, driver => new ExerciseSlidePage(driver) },
			{PageType.SolutionsPage, driver => new SolutionsPage(driver) },
			{PageType.StartPage, driver => new StartPage(driver) },
			{PageType.QuizSlidePage, driver => new QuizSlidePage(driver) },
			{PageType.RegistrationPage, driver => new RegistrationPage(driver) },
			{PageType.IncomprehensibleType, driver => new UlearnPage(driver) },
		};

		public UlearnPage CastTo(PageType pageType)
		{
			return PageFabric[pageType](driver);
		}

		public string GetUserName()
		{
			var element = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.UserNameXPath));
			return element == null ? null : element.Text;
		}
	}
}
