using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Selenium.UlearnDriver.PageObjects
{
	public class Toc
	{
		private const string lectionXPath = "/li";

		private readonly IWebElement element;
		private readonly IWebDriver driver;
		private readonly Dictionary<string, Lazy<TocUnit>> units;

		private Dictionary<string, TocUnit> statistics;

		public Toc(IWebDriver driver, IWebElement element, string XPath)
		{
			this.driver = driver;
			this.element = element;
			var newXPath = XPath + lectionXPath;
			var unitsElements = element.FindElements(By.XPath(newXPath)).ToList();
			units = new Dictionary<string, Lazy<TocUnit>>();
			statistics = new Dictionary<string, TocUnit>();
			for (var i = 0; i < unitsElements.Count; i++)
			{
				var unitName = unitsElements[i].Text.Split(new []{"\r\n"}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
				if (unitName == null)
					throw new Exception(string.Format("Юнит с номером {0} в курсе {1} не имеет названия", i, driver.Title));
				if (unitName == "Total statistics" || unitName == "Users statistics" || unitName == "Personal statistics")
					statistics.Add(unitName, new TocUnit(driver, unitsElements[i], newXPath, i));
				else
				{
					var index = i;
					units.Add(unitName, new Lazy<TocUnit>(() => new TocUnit(driver, unitsElements[index], newXPath, index + 1)));
				}
			}
		}

		public string[] GetUnitsNames()
		{
			return units.Keys.ToArray();
		}

		public TocUnit GetUnitControl(string unitName)
		{
			return units[unitName].Value;
		}
	}
}
