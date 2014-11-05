using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.PageObjects
{
	public class TOC
	{
		private const string lectionXPath = "/li";

		private readonly IWebElement element;
		public List<TOCLection> Lections { get; private set; }

		public TOC(IWebElement element, string XPath)
		{
			this.element = element;
			var newXPath = XPath + lectionXPath;
			var lections = element.FindElements(By.XPath(newXPath)).ToList();
			Lections = new List<TOCLection>();
			for (var i = 0; i < lections.Count; i++)
				Lections.Add(new TOCLection(lections[i], newXPath, i));
		}

		public void CLick()
		{
			element.Click();
		}
	}
}
