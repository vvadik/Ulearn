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

		public UlearnPage CastTo(PageType pageType)
		{
			if (pageType == PageType.SignInPage)
				return new SignInPage(driver, parent);

			if (pageType == PageType.SlidePage)
				return new SlidePage(driver, parent);

			if (pageType == PageType.ExerciseSlidePage)
				return new ExerciseSlidePage(driver, parent);

			if (pageType == PageType.SolutionsPage)
				return new SolutionsPage(driver, parent);

			if (pageType == PageType.StartPage)
				return new StartPage(driver, parent);

			if (pageType == PageType.QuizSlidePage)
				return new QuizSlidePage(driver, parent);

			return this;
		}

		public string GetUserName()
		{
			var element = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.UserNameXPath));
			return element == null ? null : element.Text;
		}
	}
}
