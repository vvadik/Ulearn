using System;
using System.Linq;
using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.Interfaces;

namespace Selenium.UlearnDriverComponents.Pages
{
	public class SignInPage : UlearnPage
	{

		public SignInPage(IWebDriver driver, IObserver parent)
			: base(driver, parent)
		{
			if (!driver.Title.Equals(Titles.SignInPageTitle))
				throw new IllegalLocatorException("Это не страница входа, это: "
								+ driver.Title);
		}

		public UlearnDriver LoginValidUser(String userName, String password)
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

			parent.Update();
			return new UlearnDriver(driver);
		}

		public UlearnDriver LoginVk()
		{
			var loginKey = driver.FindElements(By.ClassName("btn")).FirstOrDefault(x => x.Text == "ВКонтакте");
			if (loginKey != null)
				loginKey.Click();
			else
				throw new NotFoundException("Не найдена кнопка входа через ВК");

			parent.Update();
			return new UlearnDriver(driver);
		}
	}
}
