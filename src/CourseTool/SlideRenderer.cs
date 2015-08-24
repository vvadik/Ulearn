using System.Collections.Generic;
using System.IO;
using System.Linq;
using uLearn.Web.Models;
using uLearn.Web.Views.Course;

namespace uLearn.CourseTool
{
	public class SlideRenderer
	{
		private readonly DirectoryInfo htmlDirectory;

		public SlideRenderer(DirectoryInfo htmlDirectory)
		{
			this.htmlDirectory = htmlDirectory;
		}

		public string RenderSlide(Course course, Slide slide)
		{
			var page = StandaloneLayout.Page(course, slide, CreateToc(course, slide), GetCssFiles(), GetJsFiles());
			return "<!DOCTYPE html>\n" + page.ToHtmlString();
		}

		private TocModel CreateToc(Course course, Slide slide)
		{
			var builder = new TocModelBuilder(GetSlideUrl, s => 0, course, slide.Index);
			return builder.CreateTocModel();
		}

		private string GetSlideUrl(Slide slide)
		{
			return slide.Index.ToString("000") + ".html";
		}

		public IEnumerable<string> GetCssFiles()
		{
			return Directory.EnumerateFiles(htmlDirectory.FullName + "/styles").Select(x => "styles/" + new FileInfo(x).Name);
		}

		private IEnumerable<string> GetJsFiles()
		{
			yield return "scripts/jquery-1.10.2.min.js";
			yield return "scripts/bootstrap.min.js";
			yield return "scripts/katex.min.js";
			yield return "scripts/jsdifflib.js";
			yield return "scripts/jquery.flexslider-min.js";
			yield return "scripts/codemirror.js";
			yield return "scripts/clike.js";
			yield return "scripts/python.js";
			yield return "scripts/show-hint.js";
			yield return "scripts/cscompleter.js";
			yield return "scripts/csharp-hint.js";
			yield return "scripts/closebrackets.js";
			yield return "scripts/matchbrackets.js";
			yield return "scripts/active-line.js";
			foreach (var slideJs in htmlDirectory.EnumerateFiles(@"scripts/slide*.js"))
				yield return @"scripts/" + slideJs.Name;
		}
	}
}
