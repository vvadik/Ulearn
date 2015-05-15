using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace UI.Tests.PageObjects.Pages
{
	public class UlearnPage
	{
		protected readonly IWebDriver driver;

		public UlearnPage(IWebDriver driver)
		{
			this.driver = driver;
			var mayBeExceptionH1 = driver.FindElements(By.XPath("html/body/span/h1")).FirstOrDefault();
			var mayBeExceptionH2 = driver.FindElements(By.XPath("html/body/span/h2")).FirstOrDefault();
			if (mayBeExceptionH1 == null) return;

			var pathToScreenshot = UlearnDriver.SaveScreenshot(driver);

			throw new Exception(mayBeExceptionH1.Text + "\r\n" + mayBeExceptionH2.Text + "\r\n" +
			                    "Sreenshot:\r\n" + pathToScreenshot);
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

		public T CastTo<T>() where T : UlearnPage
		{
			return PageFabric[PageTypeValue.GetTypeValue(typeof(T))](driver) as T;
		}
	}
}
