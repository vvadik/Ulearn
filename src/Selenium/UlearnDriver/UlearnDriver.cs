using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Selenium.UlearnDriver.PageObjects;
using Selenium.UlearnDriver.Pages;
using uLearn;
using uLearn.Web.DataContexts;

namespace Selenium.UlearnDriver
{
	public class UlearnDriver
	{
		private readonly IWebDriver driver;
		private readonly UlearnPage currentPage;
		private Toc toc;
		private static readonly ULearnDb db = new ULearnDb();
		private static readonly CourseManager courseManager = new CourseManager(new DirectoryInfo(@"C:\Users\213\Desktop\GitHub\uLearn\src\uLearn.Web"));
		private readonly string currentUserName;
		private string currentUserId;

		public UlearnDriver(IWebDriver driver)
		{
			this.driver = driver;
			currentPage = new UlearnPage(driver);
			var pageType = currentPage.GetPageType();
			currentUserName = currentPage.GetUserName();
			currentUserId = db.Users.First(x => x.UserName == currentUserName).Id;
			currentPage = currentPage.CastTo(pageType);

			BuildTOC(pageType);

			//var currentSlideName = toc.GetCurrentSlideName();
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
			return GoToSignInPage().LoginValidUser(Admin.Login, Admin.Password).GoToCourse(courseTitle);
		}

		public UlearnDriver LoginAndGoToCourse(string courseTitle, string login, string password)
		{
			return GoToSignInPage().LoginValidUser(login, password).GoToCourse(courseTitle);
		}

		public UlearnDriver LoginVkAndGoToCourse(string courseTitle)
		{
			return GoToSignInPage().LoginVk().GoToCourse(Titles.BasicProgrammingTitle);
		}

		public static bool HasCss(IWebElement webElement, string css)
		{
			if (webElement == null)
				return false;
			try
			{
				webElement.GetCssValue(css);
				return true;
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
