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
		private readonly IWebDriver driver;
		public Dictionary<string, TOCUnit> Units { get; private set; }

		private Dictionary<string, TOCUnit> statistics;

		public TOC(IWebDriver driver, IWebElement element, string XPath)
		{
			this.driver = driver;
			this.element = element;
			var newXPath = XPath + lectionXPath;
			var units = element.FindElements(By.XPath(newXPath)).ToList();
			Units = new Dictionary<string, TOCUnit>();
			statistics = new Dictionary<string, TOCUnit>();
			for (var i = 0; i < units.Count; i++)
			{
				var unitName = units[i].Text.Split('\n').FirstOrDefault();
				if (unitName == null)
					throw new Exception(string.Format("Юнит с номером {0} не имеет названия", i));
				if (unitName == "Total statistics" || unitName == "Users statistics" || unitName == "Personal statistics")
					statistics.Add(unitName, new TOCUnit(driver, units[i], newXPath, i));
				else
					Units.Add(unitName, new TOCUnit(driver, units[i], newXPath, i));
			}
		}
	}
}
