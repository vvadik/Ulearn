using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using Selenium.UlearnDriverComponents.Interfaces;
using Selenium.UlearnDriverComponents.PageObjects;
using Selenium.UlearnDriverComponents.Pages;
using uLearn;
using uLearn.Web.DataContexts;

namespace Selenium.UlearnDriverComponents
{
	public class UlearnDriver : IDisposable
	{
		private readonly IWebDriver driver;

		public UlearnDriver(IWebDriver driver)
		{
			this.driver = driver;
		}

		public PageType GetCurrentPageType()
		{
			var title = driver.Title;
			if (title == Titles.RegistrationPageTitle)
				return PageType.RegistrationPage;
			if (title == Titles.StartPageTitle)
				return PageType.StartPage;
			if (title == Titles.SignInPageTitle)
				return PageType.SignInPage;
			if (FindElementSafely(driver, By.ClassName("side-bar")) == null)
				return PageType.IncomprehensibleType;
			var element = FindElementSafely(driver, By.ClassName("page-header"));
			if (element != null && element.Text == "Решения")
				return PageType.SolutionsPage;
			if (FindElementSafely(driver, ElementsClasses.RunSolutionButton) != null)
				return PageType.ExerciseSlidePage;
			if (FindElementSafely(driver, By.ClassName("quiz")) != null)
				return PageType.QuizSlidePage;
			return PageType.SlidePage;
		}

		public T Get<T>() where T : UlearnPage
		{
			return GetClassObject<T>() as T;
		}

		public T GetPageObject<T>() where T : PageObject
		{
			return GetClassObject<T>() as T;
		}

		private object GetClassObject<T>()
		{
			var type = typeof (T);
			var constructors = type.GetConstructors();
			var constructor = constructors.FirstOrDefault();
			if (constructor == null)
				throw new Exception(String.Format("For type {0} constructor is not implemented", type.Name));
			var classObject = constructor.Invoke(new object[] {driver});
			return classObject;
		}

		public bool IsLogin
		{
			get {return GetUserName() != null;}
		}

		private string GetUserName()
		{
			var element = FindElementSafely(driver, By.XPath(XPaths.UserNameXPath));
			return element == null ? null : element.Text;
		}

		public string GetCurrentUserName()
		{
			var currentUserName = GetUserName();
			if (currentUserName != null)
				currentUserName = currentUserName.Replace("Здравствуй, ", "").Replace("!", "");
			if (currentUserName == null)
				throw new Exception("You are not login");
			return currentUserName;
		}

		public RegistrationPage GoToRegistrationPage()
		{
			var registrationHeaderButton = FindElementSafely(driver, By.XPath(XPaths.RegistrationHeaderButton));
			if (registrationHeaderButton == null)
				throw new NotFoundException();
			registrationHeaderButton.Click();
			return new RegistrationPage(driver);
		}

		public IToc GetToc()
		{
			return new Toc(driver);
		}

		public IEnumerable<TeX> TeX
		{
			get
			{
				var texs = FindElementsSafely(driver, By.XPath(XPaths.TexXPath));
				return texs.Select((tex, i) => GetTeX(i));
			}
		}

		private TeX GetTeX(int i)
		{
			var element = FindElementSafely(driver, By.XPath(XPaths.GetRenderTexXPath(i)));
			var tex =  new TeX(element != null);
			return tex;
		}

		public StartPage GoToStartPage()
		{
			driver.Navigate().GoToUrl(ULearnReferences.StartPage);
			return new StartPage(driver);
		}

		private SignInPage GoToSignInPage()
		{
			driver.Navigate().GoToUrl(ULearnReferences.StartPage);

			var startPage = new StartPage(driver);
			var signInPage = startPage.GoToSignInPage();
			return signInPage;
		}

		public SlidePage LoginAdminAndGoToCourse(string courseTitle)
		{
			var startPage = GoToSignInPage().LoginValidUser(Admin.Login, Admin.Password);
			return startPage.GoToCourse(courseTitle);
		}

		public SlidePage LoginAndGoToCourse(string courseTitle, string login, string password)
		{
			var startPage = GoToSignInPage().LoginValidUser(login, password);
			return startPage.GoToCourse(courseTitle);
		}

		public SlidePage LoginVkAndGoToCourse(string courseTitle)
		{
			var startPage = GoToSignInPage().LoginVk();
			return startPage.GoToCourse(Titles.BasicProgrammingTitle);
		}

		public IEnumerable<SlidePage> EnumeratePages(string course, string userName, string password)
		{
			if (!IsLogin)
				GoToSignInPage().LoginValidUser(userName, password);
			driver.Navigate().GoToUrl("https://localhost:44300/Course/" + course + "/Slide/" + 0);
			while (true)
			{
				if (GetCurrentPageType() == PageType.SolutionsPage)
				{
					var solutionsPage = Get<SolutionsPage>();
					if (!solutionsPage.GetNavArrows().HasNextButton())
						yield break;
					solutionsPage.GetNavArrows().ClickNextButton();
					continue;
				}
				SlidePage page = Get<SlidePage>();
				yield return page;
				if (!page.GetNavArrows().HasNextButton())
					yield break;
				GoToNextSlide(page);
			}
		}

		private static void GoToNextSlide(SlidePage page)
		{
			if (!page.GetNavArrows().IsActiveNextButton())
				page.GetRateBlock().RateSlide(Rate.Good);
			page.GetNavArrows().ClickNextButton();
		}

		public static bool HasCss(IWebElement webElement, string css)
		{
			if (webElement == null)
				return false;
			try
			{
				return webElement.GetAttribute("class").Contains(css);//.GetCssValue(css);
				//return true;
			}
			catch (StaleElementReferenceException)
			{
				return false;
			}
		}

		public static IWebElement FindElementSafely(IWebDriver driver, By by)
		{
			try
			{
				var element = driver.FindElement(by);
				return element;
			}
			catch
			{
				return null;
			}
		}

		public static List<IWebElement> FindElementsSafely(IWebDriver driver, By by)
		{
			try
			{
				var elements = driver.FindElements(by);
				return elements.ToList();
			}
			catch
			{
				return null;
			}
		}

		public static readonly Dictionary<string, string> factory = new Dictionary<string, string>
		{
			{Titles.BasicProgrammingTitle, "BasicProgramming"}
		};

		/// <summary>
		/// Конвертирует заголовок страницы (названия курса) в название курса из CourseManager
		/// </summary>
		/// <param name="courseName">имя курса, которое можно получить со страницы слайда, путём использования свойства driver.Title</param>
		public static string ConvertCourseTitle(string courseName)
		{
			if (factory.ContainsKey(courseName))
				return factory[courseName];
			throw new NotImplementedException(string.Format("Для курса {0} не определено", courseName));
		}

		public string Url
		{
			get { return driver.Url; }
		}

		public void GoToUrl(string url)
		{
			driver.Navigate().GoToUrl(url);
		}

		public void Dispose()
		{
			try
			{
				driver.Dispose();
			}
			catch
			{
				// ignored
			}
		}
	}
}
