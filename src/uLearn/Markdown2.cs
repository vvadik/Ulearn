using System;
using System.Collections.Generic;
using MarkdownDeep;
using NUnit.Framework;

namespace uLearn
{
	public class Markdown2 : Markdown
	{
		private readonly string baseUrl;
		private readonly bool checkUrl;
		public readonly List<string> localUrls;

		public Markdown2(string baseUrl, bool checkUrl = true)
		{
			localUrls = new List<string>();
			this.checkUrl = checkUrl;
			this.baseUrl = baseUrl;
			if (baseUrl != null && !baseUrl.EndsWith("/") && checkUrl)
				this.baseUrl += "/";
		}

		public override string OnQualifyUrl(string url)
		{
			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
			{
				localUrls.Add(url);
				if (!checkUrl)
					url = url.Replace("/", "_");
			}
			if (baseUrl != null && !url.StartsWith("/") && !url.Contains(":"))
				url = baseUrl + url;
			return base.OnQualifyUrl(url);
		}
	}
}