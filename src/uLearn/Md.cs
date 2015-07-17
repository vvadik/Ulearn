using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using MarkdownDeep;
using NUnit.Framework;

namespace uLearn
{
	public static class Md
	{
		public static string RenderMd(this string md, FileInfo sourceFile)
		{
			var baseUrl = CourseUnitUtils.GetDirectoryRelativeWebPath(sourceFile);
			return md.RenderMd(baseUrl);
		}

		public static HtmlString RenderTex(this string textWithTex)
		{
			var texReplacer = new TexReplacer(textWithTex);
			string html = HttpUtility.HtmlEncode(texReplacer.ReplacedText);
			return new HtmlString(texReplacer.PlaceTexInsertsBack(html));
		}

		public static string RenderMd(this string md, string baseUrl = null)
		{
			var texReplacer = new TexReplacer(md);

			var markdown = new Markdown2(baseUrl)
			{
				NewWindowForExternalLinks = true,
				ExtraMode = true,
				SafeMode = false,
				MarkdownInHtml = false
			};

			var html = markdown.Transform(texReplacer.ReplacedText);
			return texReplacer.PlaceTexInsertsBack(html);
		}

		public static Tuple<string, List<string>> GetHtmlWithUrls(this string md, string baseUrl = null)
		{
			var texReplacer = new EdxTexReplacer(md);

			var markdown = new Markdown2(baseUrl, false)
			{
				NewWindowForExternalLinks = true,
				ExtraMode = true,
				SafeMode = false,
				MarkdownInHtml = false,
			};
			
			var html = markdown.Transform(texReplacer.ReplacedText);
			
			return Tuple.Create(texReplacer.PlaceTexInsertsBack(html), markdown.localUrls);
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

		[Test]
		public void dot_emphasize_in_html2()
		{
			Assert.AreEqual(
				@"<p><span class=""tex"">noise_V, noise_{\omega}</span></p>",
				@"<span class=""tex"">noise_V, noise_{\omega}</span>".RenderMd("/").Trim());
		}
		[Test]
		public void render_tex()
		{
			Assert.AreEqual(
				@"<p>a <span class='tex'>x</span> b</p>",
				@"a $x$ b".RenderMd("/").Trim());
		}
		[Test]
		public void render_tex1()
		{
			Assert.AreEqual(
				@"<p><span class='tex'>x</span></p>",
				@"$x$".RenderMd("/").Trim());
		}
		[Test]
		public void dont_render_not_separate_dollar()
		{
			Assert.AreEqual(
				@"<p>1$=2$</p>",
				@"1$=2$".RenderMd("/").Trim());
		}

		[Test]
		public void render_tex_with_spaces_inside()
		{
			Assert.AreEqual(
				@"<p>1 <span class='tex'> = 2 </span></p>",
				@"1 $ = 2 $".RenderMd("/").Trim());
		}
		[Test]
		public void render_md_vs_tex()
		{
			Assert.AreEqual(
				@"<p><span class='tex'>a_1=b_2</span></p>",
				@"$a_1=b_2$".RenderMd("/").Trim());
		}
		[Test]
		public void dont_markdown()
		{
			Assert.AreEqual(
				"<div>\n*ha*</div>",
				@"<div markdown='false'>*ha*</div>".RenderMd("/").Trim());
		}
		
		[Test]
		public void render_complex_tex()
		{
			Assert.AreEqual(
				@"<p><span class='tex'>\rho\subset\Sigma^*\times\Sigma^*</span></p>",
				@"$\rho\subset\Sigma^*\times\Sigma^*$".RenderMd("/").Trim());
		}

		[Test]
		public void render_tex_div()
		{
			Assert.AreEqual(
				@"<div class='tex'>\displaystyle x_1=y_1</div>",
				@"$$x_1=y_1$$".RenderMd("/").Trim());
		}

		[Test]
		public void render_tex_div_and_span()
		{
			Assert.AreEqual(
				@"<div class='tex'>\displaystyle x_1=y_1</div><p> <span class='tex'>1</span></p>",
				@"$$x_1=y_1$$ $1$".RenderMd("/").Trim());
		}
		[Test]
		public void render_tex_div_and_div()
		{
			Assert.AreEqual(
				@"<div class='tex'>\displaystyle 1</div><div class='tex'>\displaystyle 2</div>",
@"$$1$$
$$2$$".RenderMd("/").Trim());
		}
		[Test]
		public void render_tex_triple_div()
		{
			Assert.AreEqual(
				@"<div class='tex'>\displaystyle 1</div><div class='tex'>\displaystyle 2</div><div class='tex'>\displaystyle 3</div>",
@"$$1$$
$$2$$
$$3$$".RenderMd("/").Trim());
		}

		[Test]
		public void render_tex_div_surrounded_by_spaces()
		{
			Assert.AreEqual(
				@"<div class='tex'>\displaystyle 1</div>",
				@" $$1$$ ".RenderMd("/").Trim());
		}		
	}
}