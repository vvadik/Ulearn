using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	public class UlearnDriver :  IUlearnDriver
	{
		private readonly IWebDriver driver;
		private UlearnPage currentPage;
		private Toc toc;
		public static readonly CourseManager courseManager = new CourseManager(new DirectoryInfo(courseManagerPath));
		private string currentUserName;
		private string currentUserId;
		private string currentSlideName;
		private string currentSlideId;
		private const string courseManagerPath = @"C:\Users\213\Desktop\GitHub\uLearn\src\uLearn.Web";
		private readonly HashSet<IObserver> observers = new HashSet<IObserver>();
		private static readonly Db db = new Db();

		public UlearnDriver(IWebDriver driver)
		{
			this.driver = driver;

			Configure();


			//DesiredCapabilities caps = DesiredCapabilities.Chrome();
			//driver.Manage()
			//LoggingPreferences logPrefs = new LoggingPreferences();
			//logPrefsenable(LogType.BROWSER, Level.ALL);
			//caps.SetCapability(CapabilityType.LOGGING_PREFS, logPrefs);
		}

		public Rate GetRateFromDb()
		{
			return db.GetRate(currentSlideId);
		}


		private void Configure()
		{
			DeterminePage();

			DetermineUser();
			BuildToc(currentPage.GetPageType());

			DetermineContentPageSettings();
		}

		public string GetCurrentSlideId()
		{
			return currentSlideId;
		}

		private void DeterminePage()
		{
			var newPage = new UlearnPage(driver);
			var newPageType = newPage.GetPageType();
			currentPage = newPage.CastTo(newPageType);
		}

		private void DetermineContentPageSettings()
		{
			var ulearnContentPage = currentPage as UlearnContentPage;
			if (ulearnContentPage != null)
				currentSlideName = ulearnContentPage.GetSlideName();
			if (currentSlideName == null)
				return;
			//commented CM
			//var currentSlide = courseManager.GetCourses().SelectMany(x => x.Slides).FirstOrDefault(x => x.Title == currentSlideName);
			//if (currentSlide != null)
			//	currentSlideId = currentSlide.Id;
		}

		private void DetermineUser()
		{
			currentUserName = currentPage.GetUserName();
			if (currentUserName != null)
			{
				currentUserName = currentUserName.Replace("Здравствуй, ", "").Replace("!", "");
				currentUserId = db.GetUserId(currentUserName);
			}
		}

		public bool IsLogin
		{
			get {return currentUserName != null;}
		}

		public string GetCurrentUserName()
		{
			if (currentUserName == null)
				throw new Exception("You are not login");
			return currentUserName;
		}

		public void GoToRegistrationPage()
		{
			var registrationHeaderButton = FindElementSafely(driver, By.XPath(XPaths.RegistrationHeaderButton));
			if (registrationHeaderButton == null)
				throw new NotFoundException();
			registrationHeaderButton.Click();
			Update();
		}

		public string GetCurrentSlideName()
		{
			return currentSlideName ?? "";
		}

		private void BuildToc(PageType pageType)
		{
			if (pageType != PageType.SignInPage && pageType != PageType.StartPage && pageType != PageType.IncomprehensibleType)
			{
				toc = new Toc(driver);
			}
			else
				toc = null;
		}

		public UlearnPage GetPage()
		{
			return currentPage;
		}

		public IToc GetToc()
		{
			if (toc == null)
				throw new NotFoundException("Toc is not found");
			return toc;
		}

		public void GoToStartPage()
		{
			driver.Navigate().GoToUrl(ULearnReferences.StartPage);
			Update();
		}

		private SignInPage GoToSignInPage()
		{
			driver.Navigate().GoToUrl(ULearnReferences.StartPage);

			var startPage = new StartPage(driver);
			var signInPage = startPage.GoToSignInPage().currentPage as SignInPage;
			if (signInPage == null)
				throw new Exception("Sign in page not found...");
			return signInPage;
		}

		public void LoginAdminAndGoToCourse(string courseTitle)
		{
			var startPage = GoToSignInPage().LoginValidUser(Admin.Login, Admin.Password).GetPage() as StartPage;
			if (startPage != null)
				startPage.GoToCourse(courseTitle);
			throw new Exception("Start page was not found...");
		}

		public void LoginAndGoToCourse(string courseTitle, string login, string password)
		{
			var startPage = GoToSignInPage().LoginValidUser(login, password).GetPage() as StartPage;
			if (startPage != null)
				startPage.GoToCourse(courseTitle);
			throw new Exception("Start page was not found...");
		}

		public void LoginVkAndGoToCourse(string courseTitle)
		{
			var startPage = GoToSignInPage().LoginVk().GetPage() as StartPage;
			if (startPage != null)
				startPage.GoToCourse(Titles.BasicProgrammingTitle);
			throw new Exception("Start page was not found...");
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

		public void AddObserver(IObserver observer)
		{
			observers.Add(observer);
		}

		public void RemoveObserver(IObserver observer)
		{
			observers.Remove(observer);
		}

		public void NotifyObservers()
		{
			foreach (var o in observers)
				o.Update();
		}

		public void Update()
		{
			Configure();
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
	}
}
