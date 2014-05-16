using System;
using MarkdownDeep;
using NUnit.Framework;

namespace SharpLessons
{
	[TestFixture]
	public class Markdown_investigation
	{
		[Test]
		public void Simple()
		{
			var markdown = new Markdown
			{
				NewWindowForExternalLinks = true
			};
			Console.WriteLine(markdown.Transform("##Header"));
			Console.WriteLine(markdown.Transform("`<T>`"));
			Console.WriteLine(markdown.Transform("[a](http://e1.ru)"));
		}
	}
}