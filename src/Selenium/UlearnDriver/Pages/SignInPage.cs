using System;
using System.Linq;
using OpenQA.Selenium;

namespace Selenium.UlearnDriver.Pages
{
	public class SignInPage : UlearnPage
	{
		//private readonly IWebDriver driver;

		public SignInPage(IWebDriver driver)
			: base(driver)
		{
			//this.driver = driver;
			if (!driver.Title.Equals(Titles.SignInPageTitle))
				throw new IllegalLocatorException("Это не страница входа, это: "
								+ driver.Title);
		}

		/// <summary>
		/// Войти как пользователь системы
		/// </summary>
		/// <param name="userName">Имя пользователя</param>
		/// <param name="password">Пароль</param>
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

		/// <summary>
		/// Войти как пользователь системы через ВК
		/// </summary>
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
