using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriverComponents;
using Selenium.UlearnDriverComponents.Interfaces;
using Selenium.UlearnDriverComponents.Pages;
using uLearn.Web.Models;

namespace Selenium.SeleniumTests
{
	[TestFixture]
	public class SlidePageShould
	{
		[Test]
		public void HasCorrectRateStatus()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				IUlearnDriver ulearnDriver = new UlearnDriver(driver);
				ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				var page = ulearnDriver.GetPage() as SlidePage;
				var isRated = page.IsSlideRated();
				var isReallyRated = ulearnDriver.GetRateFromDb() != Rate.NotWatched;
				Assert.AreEqual(isReallyRated, isRated);
			}
		}

		[Test]
		public void RateCorrect()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				IUlearnDriver ulearnDriver = new UlearnDriver(driver);
				ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				var page = ulearnDriver.GetPage() as SlidePage;
				if (!page.IsSlideRated())
					page.RateSlide(Rate.Good);
				var rate = page.GetCurrentRate();
				var realRate = ulearnDriver.GetRateFromDb();
				Assert.AreEqual(realRate, rate);
			}
		}
	}
}
