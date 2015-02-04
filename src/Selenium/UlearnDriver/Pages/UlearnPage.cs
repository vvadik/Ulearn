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

		public PageType GetPageType()
		{
			var title = GetTitle();
			if (title == Titles.StartPageTitle)
				return PageType.StartPage;
			if (title == Titles.SignInPageTitle)
				return PageType.SignInPage;
			if (driver.FindElement(By.ClassName("side-bar")) != null)
			{
				var element = driver.FindElement(By.ClassName("page-header"));
				if (element != null && element.Text == "Решения")
					return PageType.SolutionsPage;
				if (driver.FindElement(By.ClassName("CodeMirror")) != null)
					return PageType.ExerciseSlidePage;
				return PageType.SlidePage;
			}
			return PageType.IncomprehensibleType;
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
	}
}
