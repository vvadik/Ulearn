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
		public void TestCreateDB()
		{
			var db = new uLearn.Web.DataContexts.ULearnDb();// ULearnDb();
			var b = db.Users.Count();
			//var a = db.AspNetUsers.Count();
			Console.Write(b);
		}
	}
}
