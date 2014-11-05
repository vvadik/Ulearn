using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.PageObjects
{
	public class TOCLection
	{
		private readonly IWebElement element;
		private const string slideXPath = "/ul/li";
		public List<IWebElement> Slides { get; private set; }
		public TOCLection(IWebElement element, string XPath, int i)
		{
			this.element = element;
			string index = string.Format("[{0}]", i);
			var newXPath = XPath + index + slideXPath;
			Slides = element.FindElements(By.XPath(newXPath)).ToList();
		}

		public void Click()
		{
			element.Click();
		}
	}
}
