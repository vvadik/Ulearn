using OpenQA.Selenium;

namespace Selenium.UlearnDriver.Pages
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

		public string GetUrl()
		{
			return driver.Url;
		}

		public string GetUserName()
		{
			var element = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.UserNameXPath));
			return element == null ? null : element.Text;
		}

		public PageType GetPageType()
		{
			var title = GetTitle();
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
				return PageType.Quiz;
			return PageType.SlidePage;
		}

		public UlearnPage CastTo(PageType pageType)
		{
			if (pageType == PageType.SignInPage)
				return new SignInPage(driver);

			if (pageType == PageType.SlidePage)
				return new SlidePage(driver);

			if (pageType == PageType.ExerciseSlidePage)
				return new ExerciseSlidePage(driver);

			if (pageType == PageType.SolutionsPage)
				return new SolutionsPage(driver);

			if (pageType == PageType.StartPage)
				return new StartPage(driver);

			return this;
		}

		public string GetSlideName()
		{
			var element = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.SlideHeaderXPath));
			return element == null ? null : element.Text;
		}
	}
}
