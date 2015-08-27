using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using uLearn.Model.Blocks;
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

		public void CopyLocalFiles(string md, string slideDir)
		{
			var urls = md.GetHtmlWithUrls("/static/").Item2;
			try
			{
				foreach (var url in urls)
					File.Copy(string.Format("{0}/{1}", slideDir, url), string.Format("{0}/static/{1}", htmlDirectory.FullName, url));
			}
			catch (Exception)
			{
			}
		}

		public string RenderSlide(Course course, Slide slide)
		{
			var page = StandaloneLayout.Page(course, slide, CreateToc(course, slide), GetCssFiles(), GetJsFiles());
			foreach (var block in slide.Blocks.OfType<MdBlock>())
				CopyLocalFiles(block.Markdown, slide.Info.SlideFile.Directory.FullName);
			return "<!DOCTYPE html>\n" + page.ToHtmlString();
		}

		public string RenderInstructorsNote(Course course, string unitName)
		{
			var note = course.FindInstructorNote(unitName);
			if (note == null)
				return null;
			var similarSlide = course.Slides.First(x => x.Info.UnitName == unitName);
			var slide = new Slide(new[] { new MdBlock(note.Markdown) }, new SlideInfo(unitName, similarSlide.Info.SlideFile, -1), "Заметки преподавателю", "1");
			var page = StandaloneLayout.Page(course, slide, CreateToc(course, slide), GetCssFiles(), GetJsFiles());
			CopyLocalFiles(note.Markdown, similarSlide.Info.SlideFile.Directory.FullName);
			return "<!DOCTYPE html>\n" + page.ToHtmlString();
		}

		private TocModel CreateToc(Course course, Slide slide)
		{
			var builder = new TocModelBuilder(GetSlideUrl, s => 0, course, slide.Index) { IsInstructor = true, GetUnitInstructionNotesUrl = x => x + ".html", GetUnitStatisticsUrl = x => "404.html" };
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
