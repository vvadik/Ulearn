using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using uLearn.Web.Ideone;
using uLearn.Web.Models;

namespace uLearn.Web
{
	[TestFixture]
	public class IdeoneSubmitAllSlide_Test
	{
		private static string NormalizeString(string s)
		{
			return s.LineEndingsToUnixStyle().Trim();
		}

		[Explicit]
		[TestCaseSource("GetSlidesTestCases")]
		public void TestSlides(ExerciseSlide slide, ExecutionService executionService)
		{
			TestExerciseSlide(slide, executionService);
		}

		public IEnumerable<TestCaseData> GetSlidesTestCases()
		{
			var executionService = new ExecutionService();
			var courseManager = CourseManager.LoadAllCourses();
			Assert.That(courseManager.Courses.Count == 2);
			foreach (var course in courseManager.Courses)
				foreach (var slide in course.Slides.OfType<ExerciseSlide>())
				{
					yield return new TestCaseData(slide, executionService).SetName(course.Id + " - " + slide.Info.UnitName + " - " + slide.Title);
				}
		}

		private static void TestExerciseSlide(ExerciseSlide slide, ExecutionService executionService)
		{
			var solution = slide.Solution.BuildSolution(slide.Solution.TemplateSolution);
			if (solution.HasErrors)
				Assert.Fail("Template solution: "+slide.Solution.TemplateSolution + "\n\n" +"sourse code: "+ solution.SourceCode + "\n\n" + "solution has error in: " +
				            slide.Info.UnitName + " - " + slide.Title +
				            "\n" + "\terror: " + solution.ErrorMessage + "\n\n");
			else
			{
				//ExperimentMethod(solution); Попытка научиться проводить тестирование, не отправляя на Ideon.
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
					Assert.Fail("mistake in: " + slide.Info.UnitName + " - " + slide.Title + "\n" +
								"\tActualOutput: " + result.ActualOutput + "\n" +
								"\tExpectedOutput: " + result.ExpectedOutput + "\n" +
								"\tCompilationError: " + result.CompilationError + "\n" +
								"\tSourceCode: " + solution.SourceCode + "\n\n");
				}
			}
		}

		private static void ExperimentMethod(SolutionBuildResult solution)
		{
			string output = "";
			//CSharpCodeProvider codeProvider = new CSharpCodeProvider();
			//var codeProvider = new CodeDomProvider();
			//ICodeCompiler icc = codeProvider.CreateCompiler(); 
			CompilerParameters parameters = new CompilerParameters();
			parameters.GenerateExecutable = true;
			parameters.OutputAssembly = output;
			var provider = CodeDomProvider.CreateProvider("CSharp");
			var results = provider.CompileAssemblyFromSource(parameters, solution.SourceCode);
			//CompilerResults results = icc.CompileAssemblyFromSource(parameters, solution.SourceCode);
			Console.WriteLine(solution.SourceCode);
			Console.WriteLine(string.Join("\n", parameters.EmbeddedResources));
			foreach (var error in results.Errors)
			{
				Console.WriteLine(error);
			}
			Assert.AreEqual(0, results.Errors.Count);
		}
	}
}