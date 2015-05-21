using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using NUnit.Framework;

namespace uLearn
{
	[TestFixture]
	public abstract class SlideTestBase
	{
		[Test]
		public void Test()
		{
			BaseCourseTests.TestExercise(GetType());
		}
	}

	[TestFixture]
	public abstract class BaseCourseTests
	{
		private readonly Type someSlideClass;

		protected BaseCourseTests(Type someSlideClass)
		{
			this.someSlideClass = someSlideClass;
		}

		[Test]
		[Category("Long")]
		public void CheckAllYoutubeVideos()
		{
			var course = new CourseLoader().LoadCourse(new DirectoryInfo(@"..\..\Slides"));
			Assert.That(course.Slides.Length, Is.GreaterThan(0));
			var webClient = new WebClient();
			var youtubeBlocks = course.Slides.SelectMany(slide => slide.Blocks.OfType<YoutubeBlock>().Select(b => new { slide, b.VideoId }));
			var videos = new HashSet<string>();
			foreach (var b in youtubeBlocks)
			{
				try
				{
					webClient.DownloadData("http://gdata.youtube.com/feeds/api/videos/" + b.VideoId);
				}
				catch (Exception)
				{
					throw new AssertionException("Incorrect video {1} on slide {0}".WithArgs(b.slide.Title, b.VideoId));
				}
				Assert.IsTrue(videos.Add(b.VideoId), "Duplicate video {1} on slide {0}".WithArgs(b.slide.Title, b.VideoId));
			}
		}

		[TestCaseSource("GetExerciseSlidesTestCases")]
		public void Slide(Type slideType)
		{
			Assert.IsTrue(typeof(SlideTestBase).IsAssignableFrom(slideType), slideType.ToString() + " does not inherit from SlideTestBase");
		}

		public IEnumerable<TestCaseData> GetExerciseSlidesTestCases()
		{
			return GetSlideTypes()
				.Select(type_attr => type_attr.Item1)
				.Where(type => GetExpectedOutputAttributes(type).Any())
				.Select(type => new TestCaseData(type).SetName(type.Name + ".Main"));
		}

		public IEnumerable<Tuple<Type, SlideAttribute>> GetSlideTypes()
		{
			return someSlideClass.Assembly
				.GetTypes()
				.Select(t => Tuple.Create(t, t.GetCustomAttributes(typeof(SlideAttribute)).Cast<SlideAttribute>().FirstOrDefault()))
				.Where(t => t.Item2 != null);
		}

		public static void TestExercise(Type slide)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			var newOut = new StringWriter();
			var oldOut = Console.Out;
			var methodInfo = slide.GetMethod("Main", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			if (methodInfo == null)
				return;
			Console.SetOut(newOut);
			try
			{
				methodInfo.Invoke(null, null);
			}
			finally
			{
				Console.SetOut(oldOut);
			}
			var declaringType = methodInfo.DeclaringType;
			if (declaringType == null) throw new Exception("should be!");
			var expectedOutput = String.Join("\n", GetExpectedOutputAttributes(declaringType).Select(a => a.Output));
			Console.WriteLine(newOut.ToString());
			Assert.AreEqual(PrepareOutput(expectedOutput), PrepareOutput(newOut.ToString()));
		}

		private static string PrepareOutput(string output)
		{
			return String.Join("\n", output.Trim().SplitToLines());
		}

		public static IEnumerable<ExpectedOutputAttribute> GetExpectedOutputAttributes(Type declaringType)
		{
			return declaringType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
				.SelectMany(m => m.GetCustomAttributes(typeof(ExpectedOutputAttribute)))
				.Cast<ExpectedOutputAttribute>();
		}
	}
}