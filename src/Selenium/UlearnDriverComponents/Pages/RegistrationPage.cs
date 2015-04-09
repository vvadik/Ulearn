using OpenQA.Selenium;

namespace Selenium.UlearnDriverComponents.Pages
{
	class RegistrationPage : UlearnPage
	{
		private IWebElement loginField;
		private IWebElement passwordField;
		private IWebElement confirmPasswordField;
		private IWebElement registerButton;

		public RegistrationPage(IWebDriver driver)
			: base(driver)
		{
			Configure();
		}

		private void Configure()
		{
			loginField = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.RegistrationNameField));
			passwordField = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.RegistrationPasswordField));
			confirmPasswordField = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.RegistrationConfirmPasswordField));
			registerButton = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.RegistrationRegisterButton));
			CheckPage();
		}

		private void CheckPage()
		{
			if (loginField == null)
				throw new NotFoundException("login field not found");
			if (passwordField == null)
				throw new NotFoundException("password field not found");
			if (confirmPasswordField == null)
				throw new NotFoundException("confirm password fiels not found");
			if (registerButton == null)
				throw new NotFoundException("registrated button not found");
		}

		public void SignUp(string login, string password)
		{
			loginField.SendKeys(login);
			passwordField.SendKeys(password);
			confirmPasswordField.SendKeys(password);
			registerButton.Click();
		}
	}
}
