using MarkdownDeep;

namespace uLearn
{
	public static class Md
	{
		public static string RenderMd(this string md)
		{
			var markdown = new Markdown
			{
				NewWindowForExternalLinks = true,
				ExtraMode = true,
				SafeMode = true,
			};
			return markdown.Transform(md);
		}
	}
}