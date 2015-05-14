using System;
using System.Linq;
using OpenQA.Selenium;

namespace UI.Tests.PageObjects.Pages
{
	public class SignInPage : UlearnPage
	{

		public SignInPage(IWebDriver driver)
			: base(driver)
		{
		}

		public StartPage LoginValidUser(String userName, String password)
		{
			var nameField = driver.FindElement(ElementsId.UserNameField);
			nameField.SendKeys(userName);
			var passField = driver.FindElement(ElementsId.UserPasswordField);
			passField.SendKeys(password);
			var loginKey = driver.FindElements(By.ClassName("btn")).FirstOrDefault(x => x.Text != "ВКонтакте");
			if (loginKey != null)
				loginKey.Click();
			else
				throw new NotFoundException("Не найдена стандартная кнопка входа");

			return new StartPage(driver);
		}

		public StartPage LoginVk()
		{
			var loginKey = driver.FindElements(By.ClassName("btn")).FirstOrDefault(x => x.Text == "ВКонтакте");
			if (loginKey != null)
				loginKey.Click();
			else
				throw new NotFoundException("Не найдена кнопка входа через ВК");

			return new StartPage(driver);
		}
	}
}
