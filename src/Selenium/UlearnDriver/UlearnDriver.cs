using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Selenium.UlearnDriver.PageObjects;
using Selenium.UlearnDriver.Pages;
using uLearn.Web.DataContexts;

namespace Selenium.UlearnDriver
{
	public class UlearnDriver
	{
		private readonly IWebDriver driver;
		private readonly UlearnPage currentPage;
		private Lazy<TOC> TOC;

		public UlearnDriver(IWebDriver driver)
		{
			this.driver = driver;
			currentPage = new UlearnPage(driver);
			var pageType = currentPage.GetPageType();
			currentPage = currentPage.CastTo(pageType);

			BuildTOC(pageType);
		}

		private void BuildTOC(PageType pageType)
		{
			if (pageType != PageType.SignInPage && pageType != PageType.StartPage && pageType != PageType.IncomprehensibleType)
				TOC = new Lazy<TOC>(() => new TOC(driver, driver.FindElement(By.XPath(XPaths.TOCXPath)), XPaths.TOCXPath));
			else
				TOC = null;
		}

		public UlearnPage GetPage()
		{
			return currentPage;
		}

		public TOC GetToc()
		{
			if (TOC.Value == null)
				throw new NotFoundException("TOC is not found");
			return TOC.Value;
		}

		//private PageType DeterminePageType()
		//{
		//	throw new NotImplementedException();
		//}
		//
		//public PageType GetPageType()
		//{
		//	return pageType;
		//}

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
	}
}
