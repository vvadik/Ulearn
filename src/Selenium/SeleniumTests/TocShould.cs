using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriverComponents;
using Selenium.UlearnDriverComponents.Interfaces;
using Selenium.UlearnDriverComponents.Pages;
using uLearn;

namespace Selenium.SeleniumTests
{
	[TestFixture]
	public class TocShould
	{
		[Test]
		public void BeNotNullInCourse()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				UlearnDriver ulearnDriver = new UlearnDriver(driver);
				ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				var toc = ulearnDriver.GetToc();
				Assert.IsNotNull(toc);
			}
		}

		[Test]
		[ExpectedException(typeof(NotFoundException))]
		public void BeNullInStartPage()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				ulearnDriver.GetToc();
				//ulearnDriver = (ulearnDriver.GetPage() as StartPage).GoToSignInPage();
				//Assert.IsNull(ulearnDriver.GetToc());
			}
		}

		[Test]
		[ExpectedException(typeof(NotFoundException))]
		public void BeNullInSignInPage()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				var ulearnDriver = new UlearnDriver(driver);
				var signInPage = ulearnDriver.Get<StartPage>().GoToSignInPage();
				ulearnDriver.GetToc();
			}
		}
	}
}
