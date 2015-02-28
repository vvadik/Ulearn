using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.PageObjects;
using Selenium.UlearnDriverComponents.Pages;
using uLearn;
using uLearn.Web.DataContexts;

namespace Selenium.UlearnDriverComponents
{
	public class UlearnDriver
	{
		private readonly IWebDriver driver;
		private readonly UlearnPage currentPage;
		private Toc toc;
		public static readonly ULearnDb db = new ULearnDb();
		public static readonly CourseManager courseManager = new CourseManager(new DirectoryInfo(courseMenegerPath));
		private string currentUserName;
		private string currentUserId;
		private string currentSlideName;
		private string currentSlideId;
		private const string courseMenegerPath = @"C:\Users\213\Desktop\GitHub\uLearn\src\uLearn.Web";

		public UlearnDriver(IWebDriver driver)
		{
			this.driver = driver;
			currentPage = new UlearnPage(driver);

			var pageType = currentPage.GetPageType();
			currentPage = currentPage.CastTo(pageType);

			DetermineUser();
			BuildTOC(pageType);


			DetermineContentPageSettings();
		}

		private void DetermineContentPageSettings()
		{
			var ulearnContentPage = currentPage as UlearnContentPage;
			if (ulearnContentPage != null)
				currentSlideName = ulearnContentPage.GetSlideName();
			if (currentSlideName == null)
				return;
			var currentSlide = courseManager.GetCourses().SelectMany(x => x.Slides).FirstOrDefault(x => x.Title == currentSlideName);
			if (currentSlide != null)
				currentSlideId = currentSlide.Id;
		}

		private void DetermineUser()
		{
			currentUserName = currentPage.GetUserName();
			if (currentUserName != null)
			{
				currentUserName = currentUserName.Replace("Здравствуй, ", "").Replace("!", "");
				currentUserId = db.Users.First(x => x.UserName == currentUserName).Id;
			}
		}

		public bool IsLogin()
		{
			return currentUserName != null;
		}

		public string GetCurrentUserName()
		{
			if (currentUserName == null)
				throw new Exception("You are not login");
			return currentUserName;
		}

		public UlearnDriver ClickRegistration()
		{
			var registrationHeaderButton = FindElementSafely(driver, By.XPath(XPaths.RegistrationHeaderButton));
			if (registrationHeaderButton == null)
				throw new NotFoundException();
			registrationHeaderButton.Click();
			return new UlearnDriver(driver);
		}

		public string GetCurrentSlideName()
		{
			return currentSlideName ?? "";
		}

		private void BuildTOC(PageType pageType)
		{
			if (pageType != PageType.SignInPage && pageType != PageType.StartPage && pageType != PageType.IncomprehensibleType)
				toc = new Toc(driver, driver.FindElement(By.XPath(XPaths.TocXPath)), XPaths.TocXPath);
			else
				toc = null;
		}

		public UlearnPage GetPage()
		{
			return currentPage;
		}

		public Toc GetToc()
		{
			if (toc == null)
				throw new NotFoundException("Toc is not found");
			return toc;
		}

		public UlearnDriver GoToStartPage()
		{
			driver.Navigate().GoToUrl(ULearnReferences.StartPage);
			return new UlearnDriver(driver);
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

		public UlearnDriver LoginAdminAndGoToCourse(string courseTitle)
		{
			var startPage = GoToSignInPage().LoginValidUser(Admin.Login, Admin.Password).GetPage() as StartPage;
			if (startPage != null)
				return startPage.GoToCourse(courseTitle);
			throw new Exception("Start page was not found...");
		}

		public UlearnDriver LoginAndGoToCourse(string courseTitle, string login, string password)
		{
			var startPage = GoToSignInPage().LoginValidUser(login, password).GetPage() as StartPage;
			if (startPage != null)
				return startPage.GoToCourse(courseTitle);
			throw new Exception("Start page was not found...");
		}

		public UlearnDriver LoginVkAndGoToCourse(string courseTitle)
		{
			var startPage = GoToSignInPage().LoginVk().GetPage() as StartPage;
			if (startPage != null)
				return startPage.GoToCourse(Titles.BasicProgrammingTitle);
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
	}
}
