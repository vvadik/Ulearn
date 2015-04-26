using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Globalization;
using Selenium.UlearnDriverComponents.Interfaces;

namespace Selenium.UlearnDriverComponents.Pages
{
	public class UlearnPage
	{
		protected readonly IWebDriver driver;

		public UlearnPage(IWebDriver driver)
		{
			this.driver = driver;
			var mayBeExceptionH1 = driver.FindElements(By.XPath("html/body/span/h1")).FirstOrDefault();
			var mayBeExceptionH2 = driver.FindElements(By.XPath("html/body/span/h2")).FirstOrDefault();
			if (mayBeExceptionH1 == null) return;

			var screenshotName = SaveScreenshot(driver);
			throw new Exception(mayBeExceptionH1.Text + "\r\n" + mayBeExceptionH2.Text + "\r\n" +
			                    "Sreenshot: " + screenshotName);
		}

		private static string SaveScreenshot(IWebDriver driver)
		{
			var screenshot = ((ITakesScreenshot) driver).GetScreenshot();
			var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
			var projectDirectory = Directory.GetCurrentDirectory();
			if (directoryInfo != null)
				projectDirectory = directoryInfo.FullName;
			var screenshotName = projectDirectory + "\\screenshots\\" +
			                     DateTime.Now.ToString(CultureInfo.InvariantCulture)
				                     .Replace("/", "").Replace(" ", "").Replace(":", "") + ".png";

			var imageBytes = Convert.FromBase64String(screenshot.ToString());

			using (var binaryWriter = new BinaryWriter(new FileStream(screenshotName, FileMode.Append, FileAccess.Write)))
			{
				binaryWriter.Write(imageBytes);
				binaryWriter.Close();
			}
			return screenshotName;
		}

		public string GetTitle()
		{
			return driver.Title;
		}

		

		private static readonly Dictionary<PageType, Func<IWebDriver, UlearnPage>> PageFabric =
			new Dictionary<PageType, Func<IWebDriver, UlearnPage>>
		{
			{PageType.SignInPage, driver => new SignInPage(driver) },
			{PageType.SlidePage, driver => new SlidePage(driver) },
			{PageType.ExerciseSlidePage, driver => new ExerciseSlidePage(driver) },
			{PageType.SolutionsPage, driver => new SolutionsPage(driver) },
			{PageType.StartPage, driver => new StartPage(driver) },
			{PageType.QuizSlidePage, driver => new QuizSlidePage(driver) },
			{PageType.RegistrationPage, driver => new RegistrationPage(driver) },
			{PageType.IncomprehensibleType, driver => new UlearnPage(driver) },
		};

		public T CastTo<T>() where T : UlearnPage
		{
			return PageFabric[PageTypeValue.GetTypeValue(typeof(T))](driver) as T;
		}
	}
}
