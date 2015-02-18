using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.Entity;
//using Microsoft.AspNet.Identity.EntityFramework;
using NUnit.Framework;
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
	}
}
