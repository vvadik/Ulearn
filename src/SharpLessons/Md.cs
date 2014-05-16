using MarkdownDeep;

namespace SharpLessons
{
	public static class Md
	{
		private static readonly Markdown markdown;

		static Md()
		{
			markdown = new Markdown
			{
				NewWindowForExternalLinks = true
			};
		}

		public static string ToHtml(string md)
		{
			return markdown.Transform(md);
		}
	}
}