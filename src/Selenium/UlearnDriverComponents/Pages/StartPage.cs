using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Selenium.UlearnDriverComponents.Pages
{
	public class StartPage : UlearnPage
	{
		private const string courseBlockClass = "col-md-6";

		public StartPage(IWebDriver driver) 
			: base(driver)
		{
			if (!driver.Title.Equals(Titles.StartPageTitle))
				throw new IllegalLocatorException("Это не стартовая страница, это: "
								+ driver.Title);
		}

		public SignInPage GoToSignInPage()
		{
			var loginLinkButton = driver.FindElement(ElementsId.SignInButton);
			loginLinkButton.Click();
			return new SignInPage(driver);
		}

		public SlidePage GoToCourse(string courseTitle)
		{
			var courseBlocks = driver.FindElements(By.ClassName(courseBlockClass)).ToList();
			if (courseTitle == Titles.BasicProgrammingTitle)
				ClickCourseButton(courseTitle, courseBlocks, 0);
			if (courseTitle == Titles.LinqTitle)
				ClickCourseButton(courseTitle, courseBlocks, 1);
			return new SlidePage(driver);
		}

		private void ClickCourseButton(string courseTitle, IList<IWebElement> courseBlocks, int index)
		{
			if (courseBlocks.Count >= index + 1)
				courseBlocks[index].FindElement(By.LinkText("Поехали!")).Click();
			else
				throw new NotFoundException(string.Format("Не найдена кнопка перехода на курс {0}", courseTitle));
		}
	}
}
