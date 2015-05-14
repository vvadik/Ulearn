using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace UI.Tests.PageObjects.Pages
{
	public class StartPage : UlearnPage
	{
		private const string courseBlockClass = "course-tile";

		public StartPage(IWebDriver driver) 
			: base(driver)
		{
		}

		public SignInPage GoToSignInPage()
		{
			var loginLinkButton = driver.FindElement(ElementsId.SignInButton);
			loginLinkButton.Click();
			return new SignInPage(driver);
		}

		public SlidePage GoToCourse(string courseTitle)
		{
			if (courseTitle == Titles.SampleCourseTitle)
				driver.Navigate().GoToUrl("https://localhost:44300/Course/SampleCourse/Slide/0");
			else
			{
				var tileButton = FindCourseTileButton(courseTitle);
				tileButton.Click();
			}
			return new SlidePage(driver);
		}

		private IWebElement FindCourseTileButton(string courseTitle)
		{
			var courseBlocks = driver.FindElements(By.ClassName(courseBlockClass)).ToList();
			var courseBlock = courseBlocks.FirstOrDefault(b => b.FindElement(By.TagName("h2")).Text == courseTitle);
			if (courseBlock != null)
				return courseBlock.FindElement(By.LinkText("Поехали!"));
			throw new NotFoundException(string.Format("Не найдена кнопка перехода на курс {0}", courseTitle));
		}
	}
}
