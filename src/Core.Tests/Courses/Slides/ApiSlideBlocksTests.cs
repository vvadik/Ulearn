using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.Courses.Slides.Blocks.Api;
using Ulearn.Core.Courses.Units;

namespace Ulearn.Core.Tests.Courses.Slides
{
	public class ApiSlideBlocksTests
	{
		private const string testDataDirectory = "Courses/Slides/TestData/";
		private XmlSlideLoader loader;
		private CourseSettings courseSettings;
		private Unit unit;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			loader = new XmlSlideLoader();
			courseSettings = new CourseSettings(CourseSettings.DefaultSettings);
			courseSettings.Scoring.Groups.Add("ScoringGroup1", new ScoringGroup { Id = "ScoringGroup1" });
			unit = new Unit(UnitSettings.CreateByTitle("Unit title", courseSettings), new DirectoryInfo(testDataDirectory));
		}

		private Slide LoadSlideFromXmlFile(string filename)
		{
			var slideFile = new DirectoryInfo(testDataDirectory).GetFile(filename);
			var slideLoadingContext = new SlideLoadingContext("CourseId", unit, courseSettings, slideFile.Directory, slideFile, 1);
			return loader.Load(slideLoadingContext);
		}

		private Slide TryLoadSlide(string fileName)
		{
			try
			{
				return LoadSlideFromXmlFile(fileName);
			}
			catch (Exception ex)
			{
				Assert.Inconclusive();
				return null;
			}
		}

		[Test]
		[TestCase("SimpleSlideWithMarkdownBlocks.xml", false, 6)]
		[TestCase("SimpleSlideWithMarkdownBlocks.xml", true, 4)]
		public void SlideWithMarkdownBlocksAndNestedNote(string fileName, bool removeHiddenBlocks, int expectedBlocksCount)
		{
			var slide = TryLoadSlide(fileName);
			var apiSlideBlocks = GetApiSlideBlocks(slide, removeHiddenBlocks);
			Assert.AreEqual(expectedBlocksCount, apiSlideBlocks.Length);
		}

		[Test]
		public void SlideWithSpoilerBlock()
		{
			var slide = TryLoadSlide("SlideWithSpoilerBlockAndMarkdown.xml");
			var apiSlideBlocks = GetApiSlideBlocks(slide, true);
			Assert.AreEqual(1, apiSlideBlocks.Length);
			var spoilerBlock = (SpoilerBlock)apiSlideBlocks[0];
			Assert.AreEqual(4, spoilerBlock.ApiBlocks.Count);
		}

		[Test]
		public void SlideWithCodeInMarkdown()
		{
			var slide = TryLoadSlide("SlideWithCodeInMarkdown.xml");
			var apiSlideBlocks = GetApiSlideBlocks(slide, true);
			Assert.AreEqual(3, apiSlideBlocks.Length);
			if (apiSlideBlocks.OfType<CodeBlock>().Any(cb => cb.Code.Contains("&lt;")))
				Assert.Fail();
		}

		[Test]
		public void SlideWithImageInMarkdown()
		{
			var slide = TryLoadSlide("SlideWithImageInMarkdown.xml");
			var apiSlideBlocks = GetApiSlideBlocks(slide, true);
			Assert.AreEqual(3, apiSlideBlocks.Length);
			var img = apiSlideBlocks[1] as ImageGalleryBlock;
			Assert.AreEqual(new []{"/TestData/manipulator.png"}, img.ImageUrls);
			
		}

		private static IApiSlideBlock[] GetApiSlideBlocks(Slide slide, bool removeHiddenBlocks)
		{
			var context = new ApiSlideBlockBuildingContext("course", slide.Id, "/TestData", removeHiddenBlocks);
			return slide.Blocks
				.SelectMany(b => b.ToApiSlideBlocks(context))
				.ToArray();
		}
	}
}