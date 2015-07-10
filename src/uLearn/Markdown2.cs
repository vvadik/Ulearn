using System;
using System.Collections.Generic;
using MarkdownDeep;
using NUnit.Framework;

namespace uLearn
{
	public class Markdown2 : Markdown
	{
		private readonly string baseUrl;
		public readonly List<string> localUrls;

		public Markdown2(string baseUrl)
		{
			localUrls = new List<string>();
			this.baseUrl = baseUrl;
			if (baseUrl != null && !baseUrl.EndsWith("/"))
				this.baseUrl += "/";
		}

		public override string OnQualifyUrl(string url)
		{
			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
				localUrls.Add(url);
			if (baseUrl != null && !url.StartsWith("/") && !url.Contains(":"))
				url = baseUrl + url;
			return base.OnQualifyUrl(url);
		}
	}
}