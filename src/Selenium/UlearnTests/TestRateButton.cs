using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriver;
using Selenium.UlearnDriver.Pages;

namespace Selenium.Tests
{
	[TestFixture]
	public class TestRateButton
	{
		private static readonly IWebDriver driver = new ChromeDriver();

		

		[Test]
		public void TestRate1()
		{
			using (driver)
			{
			}
		}
	}
}
