using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace uLearn.Web.SELENIUM_TESTS_1
{
	public class FirstTestClass
	{
		public void FirstTestMethod()
		{
			IWebDriver driver = new ChromeDriver();
			//IWebDriver driver = new FirefoxDriver();
			driver.Navigate().GoToUrl("https://localhost:44300/");
			driver.Manage().Window.Maximize();
			var element = driver.FindElement(By.LinkText("Поехали!"));
			element.Click();
			var nameField = driver.FindElement(By.Id("UserName"));
			nameField.SendKeys("admin");
			var passField = driver.FindElement(By.Id("Password"));
			passField.SendKeys("fullcontrol");
			var loginKey = driver.FindElements(By.ClassName("btn")).FirstOrDefault(x => x.Text != "ВКонтакте");
			loginKey.Click();

			driver.Close();
		}
	}
}