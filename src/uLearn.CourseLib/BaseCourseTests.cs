using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using RunCsJob;
using RunCsJob.Api;
using uLearn.Model.Blocks;

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
		public void NoDuplicateVideos()
		{
			var videos = GetVideos().ToLookup(d=> d.Arguments[1], d => d.Arguments[0]);
			foreach (var g in videos.Where(g => g.Count() > 1))
				Assert.Fail("Duplicate videos on slides " + string.Join(", ", g));
		}

		[Test]
		public void NoSpellCheckErrors()
		{
			var course = new CourseLoader().LoadCourse(new DirectoryInfo(@"..\..\Slides"));
			Assert.IsEmpty(course.SpellCheck());
		}

		[TestCaseSource("GetExerciseSlidesTestCases")]
		public void Slide(Type slideType)
		{
			Assert.IsTrue(typeof(SlideTestBase).IsAssignableFrom(slideType), slideType + " does not inherit from SlideTestBase");
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
			if (declaringType == null)
				throw new Exception("should be!");
			var expectedOutput = string.Join("\n", GetExpectedOutputAttributes(declaringType).Select(a => a.Output));
			Console.WriteLine(newOut.ToString());
			Assert.AreEqual(PrepareOutput(expectedOutput), PrepareOutput(newOut.ToString()));
		}

		private static string PrepareOutput(string output)
		{
			return string.Join("\n", output.Trim().SplitToLines());
		}

		public static IEnumerable<ExpectedOutputAttribute> GetExpectedOutputAttributes(Type declaringType)
		{
			return declaringType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
				.SelectMany(m => m.GetCustomAttributes(typeof(ExpectedOutputAttribute)))
				.Cast<ExpectedOutputAttribute>();
		}

		public IEnumerable<TestCaseData> GetVideos()
		{
			var course = new CourseLoader().LoadCourse(new DirectoryInfo(@"..\..\Slides"));
			Assert.That(course.Slides.Length, Is.GreaterThan(0));
			return course.Slides
				.SelectMany(slide =>
					slide.Blocks.OfType<YoutubeBlock>()
						.Select(b => new TestCaseData(slide.Info.SlideFile.Name, b.VideoId)));
		}

		[TestCaseSource("GetVideos")]
		[Category("Long")]
		public void CheckAllYoutubeVideos(string slideName, string videoId)
		{
			var url = "https://www.youtube.com/oembed?format=json&url=http://www.youtube.com/watch?v=" + videoId;
			new WebClient().DownloadData(url);
		}

		[TestCaseSource("GetSlidesTestCases")]
		public void EthalonSolutions_for_Exercises(ExerciseSlide slide)
		{
			var solution = slide.Exercise.Solution.BuildSolution(slide.Exercise.EthalonSolution);
			if (solution.HasErrors)
				FailOnError(slide, solution);
			else
			{
				var submission = new RunnerSubmition()
				{
					Code = solution.SourceCode,
					Id = slide.Id,
					Input = "",
					NeedRun = true
				};
				var result = SandboxRunner.Run(submission);
				var output = result.GetOutput().NormalizeEoln();
				var isRightAnswer = output.NormalizeEoln().Equals(slide.Exercise.ExpectedOutput.NormalizeEoln());
				if (!isRightAnswer)
				{
					Assert.Fail("mistake in: " + slide.Info.UnitName + " - " + slide.Title + "\n" +
								"\tActualOutput: " + output + "\n" +
								"\tExpectedOutput: " + slide.Exercise.ExpectedOutput.NormalizeEoln() + "\n" +
								"\tCompilationError: " + result.CompilationOutput + "\n" +
								"\tSourceCode: " + solution.SourceCode + "\n\n");
				}
			}
		}

		private static void FailOnError(ExerciseSlide slide, SolutionBuildResult solution)
		{
			Assert.Fail(@"Template solution: {0}

source code: {1}

solution has error in: {2} - {3}

error: {4}",
				slide.Exercise.EthalonSolution, solution.SourceCode, slide.Info.UnitName, slide.Title, solution.ErrorMessage);
		}

		public IEnumerable<TestCaseData> GetSlidesTestCases()
		{
			var course = new CourseLoader().LoadCourse(new DirectoryInfo(@"..\..\Slides"));
			return
				from slide in course.Slides.OfType<ExerciseSlide>()
				select new TestCaseData(slide).SetName(course.Id + " - " + slide.Info.UnitName + " - " + slide.Title);
		}

	}
}