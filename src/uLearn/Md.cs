using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

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
			
			var relativeUrls = new List<string>();
			markdown.RelativeUrl += relativeUrls.Add;

			var html = markdown.Transform(texReplacer.ReplacedText);
			
			return Tuple.Create(texReplacer.PlaceTexInsertsBack(html), relativeUrls);
		}
	}
}