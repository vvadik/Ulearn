using System;

namespace UI.Tests.Core
{
	public class FindByTextAttribute : Attribute
	{
		public string Text { get; private set; }

		public FindByTextAttribute(string text)
		{
			Text = text;
		}
	}
}