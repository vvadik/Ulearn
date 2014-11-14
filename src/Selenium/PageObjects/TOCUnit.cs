using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.PageObjects
{
	public class TOCUnit
	{
		private readonly IWebElement element;
		private const string slideXPath = "/ul/li";
		private IWebDriver driver;
		public List<IWebElement> Slides { get; private set; }
		public TOCUnit(IWebDriver driver, IWebElement element, string XPath, int i)
		{
			this.driver = driver;
			this.element = element;
			var index = string.Format("[{0}]", i);
			var newXPath = XPath + index + slideXPath;
			Slides = element.FindElements(By.XPath(newXPath)).ToList();
		}

		public TOC Click()
		{
			element.Click();
			var TOCElement = driver.FindElement(By.XPath(XPaths.TOCXPath));
			return new TOC(driver, TOCElement, XPaths.TOCXPath);
		}
	}
}
