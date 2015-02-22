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
		private readonly Dictionary<string, TocUnit> units;
		private readonly Dictionary<string, UnitInitInfo> initInfo;

		private Dictionary<string, TocUnit> statistics;

		public Toc(IWebDriver driver, IWebElement element, string XPath)
		{
			this.driver = driver;
			this.element = element;
			var newXPath = XPath + lectionXPath;
			var unitsElements = element.FindElements(By.XPath(newXPath)).ToList();
			units = new Dictionary<string, TocUnit>();
			statistics = new Dictionary<string, TocUnit>();
			initInfo = new Dictionary<string, UnitInitInfo>();
			for (var i = 0; i < unitsElements.Count; i++)
			{
				var unitName = unitsElements[i].Text.Split(new []{"\r\n"}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
				if (unitName == null)
					throw new Exception(string.Format("Юнит с номером {0} в курсе {1} не имеет названия", i, driver.Title));
				if (unitName == "Total statistics" || unitName == "Users statistics" || unitName == "Personal statistics")
					statistics.Add(unitName, new TocUnit(driver, unitsElements[i], newXPath, i));
				else
				{
					var isCollapsed = UlearnDriver.HasCss(driver.FindElement(By.XPath(XPaths.UnitInfoXPath(i))), "in");
					var index = i;
					units.Add(unitName, null);//new Lazy<TocUnit>(() => ));
					initInfo.Add(unitName, new UnitInitInfo(unitsElements[index], newXPath, index + 1, isCollapsed));
				}
			}
		}

		public string[] GetUnitsNames()
		{
			return units.Keys.ToArray();
		}

		public TocUnit GetUnitControl(string unitName)
		{
			if (units.Keys.All(x => x != unitName))
				throw new Exception(String.Format("slide with name {0} does not exist", unitName));
			if (units[unitName] != null)
				return units[unitName];
			var unitInfo = initInfo[unitName];
			units[unitName] = new TocUnit(driver, unitInfo.UnitElement, unitInfo.XPath, unitInfo.Iindex);
			return units[unitName];
		}

		public string GetCurrentSlideName()
		{
			return units
				.Where(x => initInfo[x.Key].IsCollapsed)
				.Select(x => x.Value)
				.Select(unit => unit
					.GetSlidesNames()
					.Select(name => new { Info = unit.GetSlideListItemInfo(name), Name = name })
					.FirstOrDefault(unitInfo => unitInfo.Info.Collapsed))
				.Where(x => x.Info != null)
				.Select(x => x.Name)
				.First();
		}
	}

	class UnitInitInfo
	{
		public UnitInitInfo(IWebElement unitElement, string xPath, int index, bool isCollapsed)
		{
			UnitElement = unitElement;
			XPath = xPath;
			Iindex = index;
			IsCollapsed = isCollapsed;
		}

		public bool IsCollapsed { get; private set; }

		public int Iindex { get; private set; }

		public string XPath { get; private set; }

		public IWebElement UnitElement { get; private set; }
	}
}
