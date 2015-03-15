using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.Interfaces;

namespace Selenium.UlearnDriverComponents.Pages
{
	public class UlearnPage
	{
		protected readonly IWebDriver driver;
		protected IObserver parent;

		public UlearnPage(IWebDriver driver, IObserver parent)
		{
			this.parent = parent;
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

		private static readonly Dictionary<PageType, Func<IWebDriver, IObserver, UlearnPage>> pageFabric =
			new Dictionary<PageType, Func<IWebDriver, IObserver, UlearnPage>>
		{
			{PageType.SignInPage, (driver, parent) => new SignInPage(driver, parent) },
			{PageType.SlidePage, (driver, parent) => new SlidePage(driver, parent) },
			{PageType.ExerciseSlidePage, (driver, parent) => new ExerciseSlidePage(driver, parent) },
			{PageType.SolutionsPage, (driver, parent) => new SolutionsPage(driver, parent) },
			{PageType.StartPage, (driver, parent) => new StartPage(driver, parent) },
			{PageType.QuizSlidePage, (driver, parent) => new QuizSlidePage(driver, parent) },
			{PageType.RegistrationPage, (driver, parent) => new RegistrationPage(driver, parent) },
			{PageType.IncomprehensibleType, (driver, parent) => new UlearnPage(driver, parent) },
		};

		public UlearnPage CastTo(PageType pageType)
		{
			return pageFabric[pageType](driver, parent);
		}

		public string GetUserName()
		{
			var element = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.UserNameXPath));
			return element == null ? null : element.Text;
		}
	}
}
