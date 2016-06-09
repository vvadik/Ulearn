using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using uLearn.Model.Blocks;
using uLearn.Model.Edx;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.CourseTool
{
	[TestFixture]
	public class CourseMonitor_Should
	{
		private Course course;
		private readonly Slide aTextSlide = new Slide(new[] { new MdBlock("hello"), }, new SlideInfo("u1", new FileInfo("file"), 0), "title", Guid.NewGuid());
		private readonly Slide exerciseSlide = new Slide(new[] { new ExerciseBlock() }, new SlideInfo("u1", new FileInfo("file"), 0), "title", Guid.Empty);

		[SetUp]
		public void SetUp()
		{
			course = new Course("id", "title", new[] {aTextSlide, exerciseSlide}, new InstructorNote[0], new CourseSettings(), new DirectoryInfo("."));
		}

		[Test]
		public void GenerateHtml()
		{
			Directory.CreateDirectory(".\\styles");
			Directory.CreateDirectory(".\\scripts");
			var content = new SlideRenderer(new DirectoryInfo("."), course).RenderSlide(aTextSlide);
			var content2 = new SlideRenderer(new DirectoryInfo("."), course).RenderSlide(exerciseSlide);
			Console.WriteLine(content);
			Console.WriteLine(content2);
		}
	}
}
