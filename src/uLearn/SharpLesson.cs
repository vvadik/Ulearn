using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownDeep;
using RazorEngine;
using SharpLessons.CSharp;
using Encoding = System.Text.Encoding;

namespace SharpLessons
{
	public class SharpLesson
	{
		private readonly string basePath;
		private readonly bool createCourseSubfolder;

		public SharpLesson(string basePath, bool createCourseSubfolder = false)
		{
			this.basePath = basePath;
			this.createCourseSubfolder = createCourseSubfolder;
			Urls.BasePath = createCourseSubfolder ? ".." : ".";
			Razor.Compile(LoadTemplate("slide"), typeof (Slide), "slide");
			Razor.Compile(LoadTemplate("course-page"), typeof (CoursePageModel), "course-page");
		}

		public void BuildCourse(string courseName, string sourcesDirectory)
		{
			List<Slide> slides = LoadSlides(sourcesDirectory).ToList();
			var resultDirectory = PrepareResultsDirectory(sourcesDirectory);
			RenderSlides(courseName, resultDirectory, slides);
			CopyStaticFiles();
		}

		private static string LoadTemplate(string templateName)
		{
			var bytes = ResourceLoader.LoadResource("templates." + templateName + ".cshtml");
			return Encoding.UTF8.GetString(bytes);
		}

		public string RenderCoursePage(CoursePageModel coursePage)
		{
			return Razor.Run("course-page", coursePage);
		}

		public string RenderSlideContent(Slide slide)
		{
			return Razor.Run("slide", slide);
		}

		private void CopyStaticFiles()
		{
			CopyAllFrom("Content");
			CopyAllFrom("Scripts");
		}

		private void CopyAllFrom(string resourcesNamespaceName)
		{
			string destDir = Path.Combine(basePath, resourcesNamespaceName);
			if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
			var files = ResourceLoader.EnumerateResourcesFrom(resourcesNamespaceName);
			foreach (var file in files)
				File.WriteAllBytes(Path.Combine(destDir, file.Filename), file.GetContent());
		}

		private string PrepareResultsDirectory(string sourcesDirectory)
		{
			var resultDirectory = basePath;
			if (createCourseSubfolder) resultDirectory = Path.Combine("results", Path.GetFileName(sourcesDirectory) ?? ":::");
			if (Directory.Exists(resultDirectory)) Directory.Delete(resultDirectory, true);
			Directory.CreateDirectory(resultDirectory);
			return resultDirectory;
		}

		private void RenderSlides(string courseName, string resultDirectory, IList<Slide> slides)
		{
			for (int iSlide = 0; iSlide < slides.Count; iSlide++)
			{
				Slide slide = slides[iSlide];
				string prevSlideId = iSlide > 0 ? slides[iSlide - 1].Id : null;
				string nextSlideId = iSlide < slides.Count - 1 ? slides[iSlide + 1].Id : null;
				string slideHtml =
					RenderCoursePage(new CoursePageModel
					{
						CourseName = courseName,
						PrevSlideId = prevSlideId,
						NextSlideIndex = nextSlideId,
						Slide = slide
					});
				string resFilename = slide.Id + ".slide.html";
				File.WriteAllText(Path.Combine(resultDirectory, resFilename), slideHtml);
			}
		}

		private IEnumerable<Slide> LoadSlides(string sourcesDirectory)
		{
			return Directory.EnumerateFiles(sourcesDirectory, "*.cs").Select(SlideParser.ParseSlide);
		}
	}
}