using OpenQA.Selenium;

namespace Selenium.UlearnDriverComponents.Pages
{
	class RegistrationPage : UlearnPage
	{
		private readonly IWebElement loginField;
		private readonly IWebElement passwordField;
		private readonly IWebElement confirmPasswordField;
		private readonly IWebElement registerButton;

		public RegistrationPage(IWebDriver driver)
			: base(driver)
		{
			loginField = UlearnDriverComponents.UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.RegistrationNameField));
			passwordField = UlearnDriverComponents.UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.RegistrationPasswordField));
			confirmPasswordField = UlearnDriverComponents.UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.RegistrationConfirmPasswordField));
			registerButton = UlearnDriverComponents.UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.RegistrationRegisterButton));
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

		public UlearnDriverComponents.UlearnDriver SignUp(string login, string password)
		{
			loginField.SendKeys(login);
			passwordField.SendKeys(password);
			confirmPasswordField.SendKeys(password);
			registerButton.Click();
			return new UlearnDriverComponents.UlearnDriver(driver);
		}
	}
}
