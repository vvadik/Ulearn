using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Selenium.PageObjects
{
	public class StartPage
	{
		private readonly IWebDriver driver;
		private const string courseBlockClass = "col-md-6";

		public StartPage(IWebDriver driver)
		{
			this.driver = driver;
			if (!driver.Title.Equals(Titles.StartPageTitle))
				throw new IllegalLocatorException("Это не стартовая страница, это: "
								+ driver.Title);

		}

		/// <summary>
		/// Войти на страницу авторизации
		/// </summary>
		public SignInPage GoToSignInPage()
		{
			var loginLinkButton = driver.FindElement(ElementsId.SignInButton);
			loginLinkButton.Click();
			return new SignInPage(driver);
		}

		/// <summary>
		/// Перейти на страницу курса
		/// </summary>
		/// <param name="courseTitle">recommended to use title from class Titles</param>
		public SlidePage GoToCourse(string courseTitle)
		{
			var courseBlocks = driver.FindElements(By.ClassName(courseBlockClass)).ToList();
			if (courseTitle == Titles.BasicProgrammingTitle)
				return ClickCourseButton(courseTitle, courseBlocks, 0);
			if (courseTitle == Titles.LinqTitle)
				return ClickCourseButton(courseTitle, courseBlocks, 1);
			throw new NotImplementedException(string.Format("Для курса {0} нет реализации в методе GoToCourse", courseTitle));
		}

		private SlidePage ClickCourseButton(string courseTitle, IList<IWebElement> courseBlocks, int index)
		{
			if (courseBlocks.Count >= index + 1)
				courseBlocks[index].FindElement(By.LinkText("Поехали!")).Click();
			else
				throw new NotFoundException(string.Format("Не найдена кнопка перехода на курс {0}", courseTitle));
			return new SlidePage(driver, courseTitle);
		}
	}
}
