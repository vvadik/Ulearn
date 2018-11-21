using System;
using System.IO;
using NUnit.Framework;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Quizzes;
using Ulearn.Core.Courses.Slides.Quizzes.Blocks;
using Ulearn.Core.Courses.Units;

namespace Ulearn.Core.Tests.Courses.Slides
{
	[TestFixture]
	public class XmlSlideLoaderTests
	{
		private const string testDataDirectory = "Courses/Slides/TestData/"; 
		
		private XmlSlideLoader loader;
		
		private CourseSettings courseSettings;
		private Unit unit;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			loader = new XmlSlideLoader();
			courseSettings = CourseSettings.DefaultSettings;
			courseSettings.Scoring.Groups.Add("ScoringGroup1", new ScoringGroup
			{
				Id = "ScoringGroup1"
			});
			unit = new Unit(UnitSettings.CreateByTitle("Unit title", courseSettings), new DirectoryInfo("."));
		}
		
		[SetUp]
		public void SetUp()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}
		
		private Slide LoadSlideFromXmlFile(string filename)
		{
			var slideFile = new DirectoryInfo(testDataDirectory).GetFile(filename);
			var slide = loader.Load(slideFile, 1, unit, "CourseId", courseSettings);
			return slide;
		}
		
		[Test]
		public void LoadEmptySlide()
		{
			var slide = LoadSlideFromXmlFile("EmptySlide.xml");

			Assert.AreEqual(0, slide.Blocks.Length);
		}
		
		[Test]
		public void LoadSimpleSlideWithMarkdownBlocks()
		{
			var slide = LoadSlideFromXmlFile("SimpleSlideWithMarkdownBlocks.xml");

			Assert.AreEqual(6, slide.Blocks.Length);
			Assert.AreEqual(Guid.Parse("C6A70E4D-9673-4D02-A50C-FE667A5BD83D"), slide.Id);
			Assert.AreEqual("Simple slide", slide.Title);
		}

		[Test]
		public void LoadEmptyQuiz()
		{
			var slide = LoadSlideFromXmlFile("EmptyQuiz.xml");
			
			Assert.AreEqual(typeof(QuizSlide), slide.GetType());
		}

		[Test]
		public void LoadQuizWithScoringSettings()
		{
			var slide = (QuizSlide)LoadSlideFromXmlFile("QuizWithScoringSettings.xml");
			
			Assert.AreEqual("ScoringGroup1", slide.Scoring.ScoringGroup);
			Assert.AreEqual(10, slide.Scoring.MaxTriesCount);
			Assert.AreEqual(true, slide.Scoring.ManualChecking);
		}

		[Test]
		public void LoadSlideWithWrongBlockShouldFail()
		{
			Assert.Catch<CourseLoadingException>(() => LoadSlideFromXmlFile("SlideWithWrongBlock.xml"));
		}

		[Test]
		public void LoadQuizWithSomeBlocks()
		{
			var slide = (QuizSlide)LoadSlideFromXmlFile("QuizWithSomeBlocks.xml");
			
			Assert.AreEqual(3, slide.Blocks.Length);
			Assert.AreEqual(2 + 3, slide.MaxScore);
			Assert.AreEqual("Внимание, вопрос!", ((FillInBlock) slide.Blocks[0]).Text);
			CollectionAssert.AreEqual(new []{ new RegexInfo { Pattern = @"\d+" } }, ((FillInBlock) slide.Blocks[0]).Regexes);
		}

		[Test]
		public void LoadEmptyExerciseShouldFail()
		{
			Assert.Catch<CourseLoadingException>(() => LoadSlideFromXmlFile("EmptyExercise.xml"));
		}
	}
}