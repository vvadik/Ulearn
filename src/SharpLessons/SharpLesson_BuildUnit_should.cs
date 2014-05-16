using System.IO;
using System.Linq;
using NUnit.Framework;

namespace SharpLessons
{
	[TestFixture]
	public class SharpLesson_BuildCourse_should
	{
		[SetUp]
		public void SetUp()
		{
			CleanUp();
		}

		[TearDown]
		public void TearDown()
		{
			CleanUp();
		}

		private readonly SharpLesson sharpLesson = new SharpLesson("out");

		private static void CleanUp()
		{
			if (Directory.Exists("out")) Directory.Delete("out", true);
			if (Directory.Exists("lesson")) Directory.Delete("lesson", true);
		}

		[Test]
		public void copy_css_and_js_to_result_path()
		{
			BuildACourse();
			Assert.That(File.Exists("out\\Content\\bootstrap.min.css"));
			Assert.That(File.Exists("out\\Scripts\\bootstrap.min.js"));
			Assert.That(File.ReadAllText("out\\Slide1.slide.html"), Is.StringContaining("./Content/bootstrap"));
		}

		[Test]
		public void make_directory()
		{
			BuildACourse();
			Assert.That(Directory.Exists("out"));
		}

		[Test]
		public void clean_directory_before_make()
		{
			Directory.CreateDirectory("out");
			File.WriteAllText("out\\badFile", "bad content");
			sharpLesson.BuildCourse("CourseName", @".\tests\lesson");
			Assert.That(!File.Exists("out\\badFile"));
		}

		[Test]
		public void make_slide_html_for_each_cs_file()
		{
			sharpLesson.BuildCourse("CourseName", @".\tests\lesson");
			Assert.That(Directory.EnumerateFiles("out", "*.slide.html").Count(),
				Is.EqualTo(Directory.EnumerateFiles(@".\tests\lesson", "*.cs").Count()));
		}

		[Test]
		public void resolve_urls_according_to_base_path()
		{
			var html = sharpLesson.RenderCoursePage(new CoursePageModel
			{
				CourseName = "Practice in Linq",
				Slide = ASlide()
			});
			Assert.That(html, Is.StringContaining("./Content"));
			Assert.That(html, Is.StringContaining("./Scripts"));
		}

		[Test]
		public void escape_code()
		{
			var codeSlide = new Slide("id", new[] {SlideBlock.FromCode("return 1<2 && 3>2;")});
			string slideHtml = sharpLesson.RenderSlideContent(codeSlide);
			Assert.That(slideHtml, Is.StringContaining("return 1&lt;2 &amp;&amp; 3&gt;2;"));
		}

		public static Slide ASlide()
		{
			return new Slide("theId", new[] {SlideBlock.FromMarkdown("hello world"), SlideBlock.FromCode("2>1")});
		}


		private void BuildACourse()
		{
			sharpLesson.BuildCourse("CourseName", @".\tests\lesson");
		}
	}
}