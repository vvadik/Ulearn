using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using uLearn.CSharp;
using uLearn.Web.Ideone;

namespace uLearn.Web.Models
{
	[TestFixture]
	public class IdeonSubmitAllSlide_test
	{
		private string NormalizeString(string s)
		{
			return s.LineEndingsToUnixStyle().Trim();
		}

		[Explicit]
		[Test]
		public void Test()
		{
			var executionService = new ExecutionService();
			var courseManager = CourseManager.LoadAllCourses();
			Assert.That(courseManager.Courses.Count == 2);
			foreach (var course in courseManager.Courses)
				foreach (var slide in course.Slides.OfType<ExerciseSlide>())
				{
					Assert.IsTrue(TestExerciseSlide(slide, executionService));
				}
		}

		[Explicit]
		[Test]
		private bool TestExerciseSlide(ExerciseSlide slide, ExecutionService executionService)
		{
			var solution = slide.Solution.BuildSolution(slide.Solution.TemplateSolution);
			if (solution.HasErrors)
				Assert.Fail(slide.Solution.TemplateSolution + "\n\n" + solution.SourceCode + "\n\n" + "solution has error in: " +
				            slide.Info.CourseName + " - " + slide.Info.UnitName + " - " + slide.Title +
				            "\n" + "\terror: " + solution.ErrorMessage + "\n\n");
			else
			{
				var submition = executionService.Submit(solution.SourceCode, "").Result;
				var output = submition.Output + "\n" + submition.StdErr;
				var isRightAnswer = NormalizeString(output).Equals(NormalizeString(slide.ExpectedOutput));
				var result = new RunSolutionResult
				{
					CompilationError = submition.CompilationError,
					IsRightAnswer = isRightAnswer,
					ExpectedOutput = slide.ExpectedOutput,
					ActualOutput = output
				};
				if (!isRightAnswer)
				{
					Assert.Fail("mistake in: " + slide.Info.CourseName + " - " + slide.Info.UnitName + " - " + slide.Title + "\n" +
					            "\tActualOutput: " + result.ActualOutput + "\n" +
					            "\tExpectedOutput: " + result.ExpectedOutput + "\n" +
					            "\tCompilationError: " + result.CompilationError + "\n" +
					            "\tSourceCode: " + solution.SourceCode + "\n\n");
				}
			}
			return true;
		}

	}
}