using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriverComponents;
using Selenium.UlearnDriverComponents.Interfaces;
using Selenium.UlearnDriverComponents.Pages;
using uLearn;
using uLearn.Web.DataContexts;

namespace Selenium.SeleniumTests
{
	[TestFixture]
	class DB_test_code
	{
		[Test]
		public void TestCreateDb()
		{
			var db = new ULearnDb();
			var userCount = db.Users.Count();
			Console.Write(userCount);
		}

		[Test]
		public void TestCourseManager()
		{
			var cm = new CourseManager(new DirectoryInfo(@"C:\Users\213\Desktop\GitHub\uLearn\src\uLearn.Web"));
			foreach (var course in cm.GetCourses())
				Console.WriteLine(course.Title); 
			var c = UlearnDriver.courseManager
					 .GetCourse(UlearnDriver.courseManager
						 .GetCourses()
						 .First(x => x.Title == "Основы программирования").Id);
			var cmUunitsName = c.GetUnits().Distinct();
			foreach (var uName in cmUunitsName)
				Console.WriteLine(uName);
		}

		
		[Test]
		[Explicit]
		public void TestWithoutObservale1()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				IUlearnDriver ulearnDriver = new UlearnDriver(driver);
				ulearnDriver = ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				var page = ulearnDriver.GetPage() as SlidePage;
				page.RateSlide(Rate.Trivial);
				page.RateSlide(Rate.Good);
				ulearnDriver.GetToc().GetUnitControl(ulearnDriver.GetToc().GetUnitsName().First()).Click();
				ulearnDriver.GetToc().GetUnitControl(ulearnDriver.GetToc().GetUnitsName().First()).Click();
				page.RateSlide(Rate.Trivial);
				page.RateSlide(Rate.Good);
			}
		}
	}
}
