using System;
using System.IO;
using System.Linq;
using uLearn;

namespace CourseEditor
{
	public class EditorModel
	{
		private SlideRenderer renderer = new SlideRenderer(new DirectoryInfo("html"));
		public DirectoryInfo CourseDirectory { get; private set; }
		public DirectoryInfo HtmlOutputDir { get { return CourseDirectory.Parent.GetOrCreateSubdir("Generated"); } }
		public Course Course { get; private set; }

		public void LoadFrom(DirectoryInfo dir)
		{
			Course = new CourseLoader().LoadCourse(dir);
			CourseDirectory = dir;
			NotifyChanged();
		}

		public void RegenerateCourse()
		{
			HtmlOutputDir.GetFiles("*.html").ToList().ForEach(f => f.Delete());
			foreach (var slide in Course.Slides)
				RegenerateSlide(slide);
		}

		public void RegenerateSlide(Slide slide)
		{
			var html = renderer.RenderSlide(Course, slide);
			File.WriteAllText(GetSlideHtmlFile(slide.Index).FullName, html);
		}

		private void NotifyChanged()
		{
			var handlers = Changed;
			if (handlers != null)
				handlers();
		}

		public event Action Changed;

		public FileInfo GetSlideHtmlFile(int slideIndex)
		{
			return HtmlOutputDir.GetFile(slideIndex.ToString("000") + ".html");
		}
	}
}