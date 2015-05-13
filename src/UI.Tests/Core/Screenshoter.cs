using System;
using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium;

namespace UI.Tests.Core
{
	public class Screenshoter
	{
		private readonly DirectoryInfo directory;

		public Screenshoter(DirectoryInfo directory)
		{
			this.directory = directory;
		}

		public string SaveScreenshot(IWebDriver driver)
		{
			if (!directory.Exists) directory.Create();
			var filename = Path.Combine(directory.FullName, DateTime.Now.ToString("s").Replace(":", "-") + ".png");
			var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
			screenshot.SaveAsFile(filename, ImageFormat.Png);
			return filename;
		}

	}
}