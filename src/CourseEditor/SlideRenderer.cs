using System.Collections.Generic;
using System.IO;
using System.Linq;
using uLearn;
using uLearn.Web.Models;
using uLearn.Web.Views.Course;
using uLearn.Web.Views.SlideNavigation;

namespace CourseEditor
{
	public class SlideRenderer
	{
		private readonly DirectoryInfo htmlDirectory;

		public SlideRenderer(DirectoryInfo htmlDirectory)
		{
			this.htmlDirectory = htmlDirectory;
		}

		public string[] GetCssFiles()
		{
			var prefix = htmlDirectory.FullName;
			return htmlDirectory.EnumerateFiles("*.css", SearchOption.AllDirectories).Select(f => f.FullName.Substring(prefix.Length + 1).Replace(@"\", @"/")).ToArray();
		}

		public string RenderSlide(Course course, Slide slide)
		{
			const string template = @"<!DOCTYPE html>
<html>
	<head>
		<meta charset='UTF-8'>
<!-- CSS -->
	</head>
	<body>
		<div class='side-bar navbar-collapse collapse navbar-nav container'>
<!-- TOC -->
		</div>
<!-- SLIDE -->
<!-- SCRIPTS -->
	</body>
</html>
";
			const string link = @"<link href='html/{0}' rel='stylesheet'/>";
			const string script = @"<script src='html/{0}'></script>";

			var styles = string.Join("\n", GetCssFiles().Select(file => string.Format(link, file)));
			var tocHtml = TableOfContents.Toc(CreateToc(course, slide)).ToHtmlString();
			var scripts = string.Join("\n", GetJsFiles().Select(file => string.Format(script, file)));
			var slideHtml = SlideHtml.Slide(slide, 0, 0).ToHtmlString();
			return template
				.Replace("<!-- CSS -->", styles)
				.Replace("<!-- TOC -->", tocHtml)
				.Replace("<!-- SLIDE -->", slideHtml)
				.Replace("<!-- SCRIPTS -->", scripts);
		}

		private TocModel CreateToc(Course course, Slide slide)
		{
			var builder = new TocModelBuilder(s => GetSlideUrl(s), s => 0, course, slide.Index);
			return builder.CreateTocModel();
		}

		private string GetSlideUrl(Slide slide)
		{
			return slide.Index.ToString("000") + ".html";
		}

		private IEnumerable<string> GetJsFiles()
		{
			yield return @"Scripts/jquery-1.10.2.min.js";
			yield return @"Scripts/bootstrap.min.js";
			yield return @"katex/katex.min.js";
			yield return @"flexslider/jquery.flexslider-min.js";
			foreach (var codemirrorJs in GetCodemirrorJs())
				yield return "codemirror/" + codemirrorJs;
			foreach (var slideJs in htmlDirectory.EnumerateFiles(@"Scripts\slide*.js"))
				yield return @"Scripts/" + slideJs.Name;
		}

		private IEnumerable<string> GetCodemirrorJs()
		{
			yield return "lib/codemirror.js";
			yield return "mode/clike/clike.js";
			yield return "mode/python/python.js";
		}
	}
}