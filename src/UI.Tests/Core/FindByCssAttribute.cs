using System;

namespace UI.Tests.Core
{
	public class FindByCssAttribute : Attribute
	{
		public string CssSelector { get; set; }

		public FindByCssAttribute(string cssSelector)
		{
			CssSelector = cssSelector;
		}
	}
}