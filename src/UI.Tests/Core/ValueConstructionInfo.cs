using System;
using System.Linq;
using OpenQA.Selenium;

namespace UI.Tests.Core
{
	public class ValueConstructionInfo
	{
		public ValueConstructionInfo(Type valueType, IWebElement[] elements, Browser browser)
		{
			Browser = browser;
			Elements = elements;
			ValueType = valueType;
		}

		public readonly Browser Browser;
		public readonly IWebElement[] Elements;
		public readonly Type ValueType;

		public ValueConstructionInfo RefineWith([CanBeNull] By criteria)
		{
			if (criteria == null)
				return this;
			return new ValueConstructionInfo(ValueType, Elements.SelectMany(e => e.FindElements(criteria)).ToArray(), Browser);
		}
	}
}