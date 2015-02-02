using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriver;

namespace Selenium.Tests
{
	[TestFixture]
	class UlearnDriverTest
	{
		private static readonly IWebDriver driver = new ChromeDriver();

		[Test]
		public void FirstTest()
		{
			var ulearnDriver = new UlearnDriver.UlearnDriver(driver);
			ulearnDriver = ulearnDriver.LoginAndGoToCourse(Titles.BasicProgrammingTitle);
			Console.WriteLine(ulearnDriver.GetPage().GetType());
		}
	}
}
