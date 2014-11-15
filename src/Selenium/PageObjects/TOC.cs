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
		private readonly Dictionary<string, TOCUnit> units;

		private Dictionary<string, TOCUnit> statistics;

		public TOC(IWebDriver driver, IWebElement element, string XPath)
		{
			this.driver = driver;
			this.element = element;
			var newXPath = XPath + lectionXPath;
			var unitsElements = element.FindElements(By.XPath(newXPath)).ToList();
			units = new Dictionary<string, TOCUnit>();
			statistics = new Dictionary<string, TOCUnit>();
			for (var i = 0; i < unitsElements.Count; i++)
			{
				var unitName = unitsElements[i].Text.Split('\n').FirstOrDefault();
				if (unitName == null)
					throw new Exception(string.Format("Юнит с номером {0} в курсе {1} не имеет названия", i, driver.Title));
				if (unitName == "Total statistics" || unitName == "Users statistics" || unitName == "Personal statistics")
					statistics.Add(unitName, new TOCUnit(driver, unitsElements[i], newXPath, i));
				else
					units.Add(unitName, new TOCUnit(driver, unitsElements[i], newXPath, i));
			}
		}

		public string[] GetUnitsNames()
		{
			return units.Keys.ToArray();
		}

		public TOCUnit GetUnitControl(string unitName)
		{
			return units[unitName];
		}
	}
}
