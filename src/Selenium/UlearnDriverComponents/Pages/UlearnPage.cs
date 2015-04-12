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
	}
}
