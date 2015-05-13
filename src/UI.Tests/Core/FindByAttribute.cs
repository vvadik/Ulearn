using System;
using OpenQA.Selenium;

namespace UI.Tests.Core
{
	public class FindByAttribute : Attribute
	{
		public string Css { get; set; }
		public string LinkText { get; set; }
		public string Id { get; set; }

		public By GetCriteria()
		{
			if (Css != null)
				return By.CssSelector(Css);
			if (LinkText != null)
				return By.LinkText(LinkText);
			throw new Exception("Must be specifid at least one criteria");
		}
	}
}