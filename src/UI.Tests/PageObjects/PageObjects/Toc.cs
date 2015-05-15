using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using UI.Tests.PageObjects.Interfaces;

namespace UI.Tests.PageObjects.PageObjects
{
	public class Toc : IToc
	{
		private readonly IWebDriver driver;

		private Dictionary<string, Lazy<ITocUnit>> units;
		private Dictionary<string, UnitInitInfo> initInfo;

		private Dictionary<string, TocUnit> statistics;

		public Toc(IWebDriver driver)
		{
			this.driver = driver;

			Configure();
		}

		private void Configure()
		{
			var unitsElements = UlearnDriver.FindElementsSafely(driver, By.XPath(XPaths.TocUnitsXPath)).ToList();
			CreateCollections();
			for (var i = 0; i < unitsElements.Count; i++)
			{
				var unitName = unitsElements[i].Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
				if (unitName == null)
					throw new Exception(string.Format("Юнит с номером {0} в курсе {1} не имеет названия", i, driver.Title));
				if (unitName == "Total statistics" || unitName == "Users statistics" || unitName == "Personal statistics")
				{
					if (!statistics.ContainsKey(unitName))
						statistics.Add(unitName, new TocUnit(driver, i));
				}
				else
				{
					if (units.ContainsKey(unitName))
						continue;
					var collapsedElement = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.UnitInfoXPath(i)));
					var isCollapsed = UlearnDriver.HasCss(collapsedElement, "collapse in");
					initInfo.Add(unitName, new UnitInitInfo(unitsElements[i], i, isCollapsed));
					units.Add(unitName, new Lazy<ITocUnit>(() => new TocUnit(driver, initInfo[unitName].Index)));
				}
			}
		}

		private void CreateCollections()
		{
			units = new Dictionary<string, Lazy<ITocUnit>>();
			statistics = new Dictionary<string, TocUnit>();
			initInfo = new Dictionary<string, UnitInitInfo>();
		}

		public string[] GetUnitsName()
		{
			return units.Keys.ToArray();
		}

		public ITocUnit GetUnitControl(string unitName)
		{
			if (units.Keys.All(x => x != unitName))
				throw new Exception(String.Format("slide with name {0} does not exist", unitName));
			return units[unitName].Value;
		}

		public bool IsCollapsed(string unitName)
		{
			return initInfo[unitName].IsCollapsed;
		}
	}

	class UnitInitInfo
	{
		public UnitInitInfo(IWebElement unitElement, int index, bool isCollapsed)
		{
			UnitElement = unitElement;
			Index = index;
			IsCollapsed = isCollapsed;
		}

		public bool IsCollapsed { get; private set; }

		public int Index { get; private set; }

		public IWebElement UnitElement { get; private set; }
	}
}
