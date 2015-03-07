using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Selenium.UlearnDriverComponents.PageObjects
{
	public class Toc : IObserver, IToc
	{
		private readonly IWebDriver driver;

		private Dictionary<string, TocUnit> units;
		private Dictionary<string, UnitInitInfo> initInfo;

		private Dictionary<string, TocUnit> statistics;
		private readonly IObserver parent;
		private readonly HashSet<IObserver> observers = new HashSet<IObserver>();

		public Toc(IWebDriver driver, IObserver parent)
		{
			this.parent = parent;
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
						statistics.Add(unitName, new TocUnit(driver, i, parent));
				}
				else
				{
					if (units.ContainsKey(unitName))
						continue;
					var collapsedElement = UlearnDriver.FindElementSafely(driver, By.XPath(XPaths.UnitInfoXPath(i)));
					var isCollapsed = UlearnDriver.HasCss(collapsedElement, "collapse in");
					units.Add(unitName, null);
					initInfo.Add(unitName, new UnitInitInfo(unitsElements[i], i, isCollapsed));
				}
			}
		}

		private void CreateCollections()
		{
			if (units == null)
				units = new Dictionary<string, TocUnit>();
			if (statistics == null)
				statistics = new Dictionary<string, TocUnit>();
			if (initInfo == null)
				initInfo = new Dictionary<string, UnitInitInfo>();
		}

		public string[] GetUnitsName()
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
			units[unitName] = new TocUnit(driver, unitInfo.Iindex, parent);
			observers.Add(units[unitName]);
			return units[unitName];
		}

		public bool IsCollapsed(string unitName)
		{
			return initInfo[unitName].IsCollapsed;
		}

		public void Update()
		{
			Configure();
			foreach (var o in observers)
				o.Update();
		}
	}

	class UnitInitInfo
	{
		public UnitInitInfo(IWebElement unitElement, int index, bool isCollapsed)
		{
			UnitElement = unitElement;
			Iindex = index;
			IsCollapsed = isCollapsed;
		}

		public bool IsCollapsed { get; private set; }

		public int Iindex { get; private set; }

		public IWebElement UnitElement { get; private set; }
	}
}
