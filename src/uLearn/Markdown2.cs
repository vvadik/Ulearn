using MarkdownDeep;

namespace uLearn
{
	public class Markdown2 : Markdown
	{
		private readonly string baseUrl;

		public Markdown2(string baseUrl)
		{
			this.baseUrl = baseUrl;
			if (baseUrl != null && !baseUrl.EndsWith("/"))
				this.baseUrl += "/";
		}

		public override string OnQualifyUrl(string url)
		{
			if (baseUrl != null && !url.StartsWith("/") && !url.Contains(":"))
				url = baseUrl + url;
			return base.OnQualifyUrl(url);
		}
	}
}