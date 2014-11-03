using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Selenium.PageObjects
{
	public class SignInPage
	{
		private readonly IWebDriver driver;

		public SignInPage(IWebDriver driver)
		{
			this.driver = driver;
			if (!driver.Title.Equals(Titles.signInPageTitle))
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
			var nameField = driver.FindElement(ElementsId.userNameField);
			nameField.SendKeys(userName);
			var passField = driver.FindElement(ElementsId.userPasswordField);
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
