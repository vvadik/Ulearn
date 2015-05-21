using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using uLearn;
using uLearn.Web.Models;
using uLearn.Web.Views.Course;

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
			var jsFiles = GetJsFiles().Select(f => Path.GetFullPath(".\\html\\" + f));
			var cssFiles = GetCssFiles().Select(f => Path.GetFullPath(".\\html\\" + f));
			var page = StandaloneLayout.Page(course, slide, CreateToc(course, slide), cssFiles, jsFiles);
			return "<!DOCTYPE html>\n" + page.ToHtmlString();
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