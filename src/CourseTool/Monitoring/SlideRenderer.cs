using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.WebPages;
using uLearn.Web.Models;
using uLearn.Web.Views.Course;
using Ulearn.Core;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.Courses.Units;

namespace uLearn.CourseTool.Monitoring
{
	/* RazorGenerator uses HelperPage which need a HelperPage.PageContext to be defined. 
	We create a fake web page for it. It's not used never but is passed to PageContext constructor. */
	public class FakeWebPage : WebPage
	{
		public override void Execute()
		{
			/* Method Execute() is never called for fake web page */
			throw new NotImplementedException();
		}
	}

	public class SlideRenderer
	{
		private readonly Course course;
		private readonly DirectoryInfo htmlDirectory;

		public SlideRenderer(DirectoryInfo htmlDirectory, Course course)
		{
			this.htmlDirectory = htmlDirectory;
			this.course = course;
			/* Create fake page context for Razor Generator */
			HelperPage.PageContext = new WebPageContext(null, new FakeWebPage(), null);
		}

		private void CopyLocalFiles(string md, string slideDir)
		{
			var urls = md.GetHtmlWithUrls("/static/").Item2;
			try
			{
				foreach (var url in urls)
				{
					var destFilepath = $"{htmlDirectory.FullName}\\static\\{url}";
					if (!File.Exists(destFilepath))
						File.Copy($"{slideDir}\\{url}", $"{htmlDirectory.FullName}\\static\\{url}");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		public void RenderSlideToFile(Slide slide, string directory)
		{
			File.WriteAllText(Path.Combine(directory, GetSlideUrl(slide)), RenderSlide(slide));
		}

		private string RenderSlide(Slide slide)
		{
			var page = StandaloneLayout.Page(course, slide, CreateToc(slide), GetCssFiles(), GetJsFiles());
			//foreach (var block in slide.Blocks.OfType<MarkdownBlock>())
			//	CopyLocalFiles(block.Markdown, slide.Info.Directory.FullName);
			return "<!DOCTYPE html>\n" + page.ToHtmlString();
		}

		public void RenderInstructorNotesToFile(Unit unit, string directory)
		{
			File.WriteAllText(
				Path.Combine(directory, GetInstructorNotesFilename(unit)),
				RenderInstructorsNote(unit)
			);
		}

		private string RenderInstructorsNote(Unit unit)
		{
			var note = unit.InstructorNote;
			if (note == null)
				return null;

			var similarSlide = unit.Slides.First();
			var slide = new Slide(new MarkdownBlock(note.Markdown))
			{
				Id = Guid.NewGuid(),
				Title = "Заметки преподавателю",
				Info = new SlideInfo(unit, similarSlide.Info.SlideFile, -1),
			};
			var page = StandaloneLayout.Page(course, slide, CreateToc(slide), GetCssFiles(), GetJsFiles());

			//CopyLocalFiles(note.Markdown, similarSlide.Info.Directory.FullName);
			return "<!DOCTYPE html>\n" + page.ToHtmlString();
		}

		private TocModel CreateToc(Slide slide)
		{
			var builder = new TocModelBuilder(GetSlideUrl, s => 0, s => s.MaxScore, (u, g) => 0, course, slide.Id)
			{
				IsInstructor = true,
				GetUnitInstructionNotesUrl = GetInstructorNotesFilename,
				GetUnitStatisticsUrl = unit => "404.html",
				IsSlideHidden = s => false
			};
			return builder.CreateTocModel();
		}

		private string GetInstructorNotesFilename(Unit unit)
		{
			return $"InstructorNotes.{unit.Url}.html";
		}

		private static string GetSlideUrl(Slide slide)
		{
			return slide.Index.ToString("000") + ".html";
		}

		private IEnumerable<string> GetCssFiles()
		{
			yield return "renderer/styles/bundle.css";
		}


		private IEnumerable<string> GetJsFiles()
		{
			yield return "renderer/scripts/bundle.js";
		}
	}
}