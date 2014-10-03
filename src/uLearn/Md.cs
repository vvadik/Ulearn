using System;
using System.IO;
using MarkdownDeep;
using NUnit.Framework;

namespace uLearn
{
	public static class Md
	{
		public static string RenderMd(this string md, FileInfo sourceFile)
		{
			var baseUrl = "/Courses/" + CourseUnitUtls.GetDirectoryRelativeWebPath(sourceFile);
			return md.RenderMd(baseUrl);
		}

		public static string RenderMd(this string md, string baseUrl = null)
		{
			var markdown = new Markdown2(baseUrl)
			{
				NewWindowForExternalLinks = true,
				ExtraMode = true,
				SafeMode = false,
				MarkdownInHtml = false,
			};
			return markdown.Transform(md);
		}

	}

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


	[TestFixture]
	public class Md_should
	{
		[Test]
		public void qualify_urls()
		{
			Assert.That("[a](a.html)".RenderMd("/Course"), Is.StringContaining("href=\"/Course/a.html\""));
			Assert.That("[a](a.html)".RenderMd("/Course/"), Is.StringContaining("href=\"/Course/a.html\""));
			Assert.That("[a](a.html)".RenderMd(), Is.StringContaining("href=\"a.html\""));
			Assert.That("[a](/a.html)".RenderMd(), Is.StringContaining("href=\"/a.html\""));
		}

		[Test]
		public void emphasize_underscore()
		{
			Assert.AreEqual(
				"<p><strong>x</strong>,</p>\n",
				new Markdown { ExtraMode = true }.Transform("__x__,"));
		}
	
		[Test]
		public void dot_emphasize_in_html()
		{
			Assert.AreEqual(
				"<p><span>_x_</span></p>\n",
				new Markdown { ExtraMode = true }.Transform("<span>_x_</span>"));
		}
	}
}