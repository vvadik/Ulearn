using System;

namespace UI.Tests.Core
{
	public class PageUrlAttribute : Attribute
	{
		public PageUrlAttribute(string url)
		{
			Url = url;
		}

		public string Url { get; private set; }
	}
}