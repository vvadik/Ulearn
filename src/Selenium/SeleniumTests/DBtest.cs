using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Selenium.UlearnDriverComponents;
using uLearn;
using uLearn.Web.DataContexts;
using uLearn.Web.Migrations;
using uLearn.Web.Models;

namespace Selenium.SELENIUM_TESTS_1
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
	}
}
