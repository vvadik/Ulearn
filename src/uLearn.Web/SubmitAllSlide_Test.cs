using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using uLearn.Web.ExecutionService;
using uLearn.Web.Models;

namespace uLearn.Web
{
	[TestFixture]
	public class SubmitAllSlide_Test
	{
		[Explicit]
		[TestCaseSource("GetSlidesTestCases")]
		public void TestSlides(ExerciseSlide slide, IExecutionService executionService)
		{
			TestExerciseSlide(slide, executionService);
		}

		private IEnumerable<TestCaseData> GetSlidesTestCases()
		{
			var executionService = new CsSandboxService();
			var courseManager = WebCourseManager.Instance;
			Assert.That(courseManager.GetCourses().Count() >= 2);
			return 
				from course in courseManager.GetCourses() 
				from slide in course.Slides.OfType<ExerciseSlide>() 
				select new TestCaseData(slide, executionService).SetName(course.Id + " - " + slide.Info.UnitName + " - " + slide.Title);
		}

		private static void TestExerciseSlide(ExerciseSlide slide, IExecutionService executionService)
		{
			var solution = slide.Solution.BuildSolution(slide.EthalonSolution);
			if (solution.HasErrors)
				Assert.Fail("Template solution: " + slide.EthalonSolution + "\n\n" + "source code: " + solution.SourceCode + "\n\n" + "solution has error in: " +
				            slide.Info.UnitName + " - " + slide.Title +
				            "\n" + "\terror: " + solution.ErrorMessage + "\n\n");
			else
			{
				//ExperimentMethod(solution); Попытка научиться проводить тестирование, не отправляя на Ideon.
				var testName = TestContext.CurrentContext.Test.Name;
				var submition = executionService.Submit(solution.SourceCode, testName).Result;
				var output = submition.StdOut + "\n" + submition.StdErr;
				var isRightAnswer = output.NormalizeEoln().Equals(slide.ExpectedOutput.NormalizeEoln());
				var result = new RunSolutionResult
				{
					CompilationError = submition.CompilationErrorMessage,
					IsRightAnswer = isRightAnswer,
					ExpectedOutput = slide.ExpectedOutput,
					ActualOutput = output
				};
				if (!isRightAnswer)
				{
					Assert.Fail("mistake in: " + slide.Info.UnitName + " - " + slide.Title + "\n" +
								"\tActualOutput: " + result.ActualOutput + "\n" +
								"\tExpectedOutput: " + result.ExpectedOutput + "\n" +
								"\tCompilationError: " + result.CompilationError + "\n" +
								"\tSourceCode: " + solution.SourceCode + "\n\n");
				}
			}
		}
	}
}