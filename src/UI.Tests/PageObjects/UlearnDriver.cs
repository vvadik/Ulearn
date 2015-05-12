using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using UI.Tests.PageObjects.Interfaces;
using UI.Tests.PageObjects.PageObjects;
using UI.Tests.PageObjects.Pages;

namespace UI.Tests.PageObjects
{
	public class UlearnDriver : IDisposable
	{
		private readonly IWebDriver driver;

		public UlearnDriver(IWebDriver driver)
		{
			this.driver = driver;
		}

		public UlearnDriver()
		{
			driver = new ChromeDriver();
			GoToStartPage();
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
			return CreateWithDriver<T>();
		}

		public T GetPageObject<T>() where T : PageObject
		{
			return CreateWithDriver<T>();
		}

		private T CreateWithDriver<T>()
		{
			var type = typeof(T);
			var constructors = type.GetConstructors();
			var constructor = constructors.FirstOrDefault();
			if (constructor == null)
				throw new Exception(String.Format("For type {0} constructor is not implemented", type.Name));
			return (T)constructor.Invoke(new object[] { driver });
		}

		public bool LoggedIn
		{
			get { return GetUserName() != null; }
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
			driver.Navigate().GoToUrl(ULearnUrls.RegistrationPage);
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
				Thread.Sleep(1000);
				var texs = FindElementsSafely(driver, By.XPath(XPaths.TexXPath));
				return texs.Select(texSpan => new TeX(texSpan));
			}
		}

		public StartPage GoToStartPage()
		{
			driver.Navigate().GoToUrl(ULearnUrls.StartPage);
			return new StartPage(driver);
		}

		private SignInPage GoToSignInPage()
		{
			driver.Navigate().GoToUrl(ULearnUrls.StartPage);

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
			if (!LoggedIn)
				GoToSignInPage().LoginValidUser(userName, password);
			return EnumeratePages(course);
		}

		public IEnumerable<SlidePage> EnumeratePages(string course)
		{
			if (titlesFactory.ContainsKey(course))
				course = titlesFactory[course];
			driver.Navigate().GoToUrl("https://localhost:44300/Course/" + course + "/Slide/" + 0);
			while (true)
			{
				var currentPageType = GetCurrentPageType();
				if (currentPageType == PageType.SolutionsPage)
				{
					var solutionsPage = Get<SolutionsPage>();
					if (!solutionsPage.GetNavArrows().HasNextButton())
						yield break;
					solutionsPage.GetNavArrows().ClickNextButton();
					continue;
				}
				var page = GetCorrectPage(currentPageType);
				yield return page;
				if (!page.GetNavArrows().HasNextButton())
					yield break;
				GoToNextSlide(page);
			}
		}

		private SlidePage GetCorrectPage(PageType currentPageType)
		{
			SlidePage page;
			switch (currentPageType)
			{
				case PageType.ExerciseSlidePage:
					page = Get<ExerciseSlidePage>();
					break;
				case PageType.QuizSlidePage:
					page = Get<QuizSlidePage>();
					break;
				default:
					page = Get<SlidePage>();
					break;
			}
			return page;
		}

		private static void GoToNextSlide(SlidePage page)
		{
			if (!page.GetNavArrows().IsActiveNextButton())
				page.GetRateBlock().RateSlide(Rate.Good);
			page.GetNavArrows().ClickNextButton();
		}

		public string SaveScreenshot()
		{
			return SaveScreenshot(driver);
		}

		public void SaveScreenshot(string fullPath)
		{
			SaveScreenshot(driver, fullPath);
		}

		/// <summary>
		/// Save screenshot and return path to it.
		/// </summary>
		/// <returns>path to screenshot</returns>
		public static string SaveScreenshot(IWebDriver driver)
		{
			var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
			var projectDirectory = Directory.GetCurrentDirectory();
			if (directoryInfo != null)
				projectDirectory = directoryInfo.FullName;
			var screenshotsDirectory = projectDirectory + "\\screenshots\\";
			if (!Directory.Exists(screenshotsDirectory))
				Directory.CreateDirectory(screenshotsDirectory);
			var screenshotName = screenshotsDirectory +
								 DateTime.Now.ToString(CultureInfo.InvariantCulture)
									 .Replace("/", "").Replace(" ", "").Replace(":", "") + ".png";

			var screenshot = ((ITakesScreenshot)driver).GetScreenshot();

			var imageBytes = Convert.FromBase64String(screenshot.ToString());

			using (var binaryWriter = new BinaryWriter(new FileStream(screenshotName, FileMode.Append, FileAccess.Write)))
			{
				binaryWriter.Write(imageBytes);
				binaryWriter.Close();
			}
			return screenshotName;
		}

		public static void SaveScreenshot(IWebDriver driver, string fullPath)
		{
			var screenshot = ((ITakesScreenshot)driver).GetScreenshot();

			var imageBytes = Convert.FromBase64String(screenshot.ToString());

			using (var binaryWriter = new BinaryWriter(new FileStream(fullPath, FileMode.Append, FileAccess.Write)))
			{
				binaryWriter.Write(imageBytes);
				binaryWriter.Close();
			}
		}

		public static bool HasCss(IWebElement webElement, string css)
		{
			if (webElement == null)
				return false;
			try
			{
				return webElement.GetAttribute("class").Contains(css);
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

		public static readonly Dictionary<string, string> titlesFactory = new Dictionary<string, string>
		{
			{Titles.BasicProgrammingTitle, "BasicProgramming"},
			{Titles.LinqTitle, "Linq"},
			{Titles.SampleCourseTitle, "SampleCourse"},
		};

		/// <summary>
		/// Конвертирует заголовок страницы (названия курса) в название курса из CourseManager
		/// </summary>
		/// <param name="courseName">имя курса, которое можно получить со страницы слайда, путём использования свойства driver.Title</param>
		public static string ConvertCourseTitle(string courseName)
		{
			if (titlesFactory.ContainsKey(courseName))
				return titlesFactory[courseName];
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
