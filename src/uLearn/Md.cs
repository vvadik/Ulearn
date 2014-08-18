using MarkdownDeep;

namespace uLearn
{
	public static class Md
	{
		public static string RenderMd(this string md)
		{
			return new Markdown
			{
				NewWindowForExternalLinks = true,
				SafeMode = true
			}
			.Transform(md);
		}
	}
}