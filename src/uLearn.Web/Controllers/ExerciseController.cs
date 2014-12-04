using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.Ideone;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class ExerciseController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly ExecutionService executionService = new ExecutionService();

		public ExerciseController()
			: this(WebCourseManager.Instance)
		{
		}

		public ExerciseController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> RunSolution(string courseId, int slideIndex = 0)
		{
			var code = GetUserCode(Request.InputStream);
			if (code.Length > TextsRepo.MAX_TEXT_SIZE)
			{
				return Json(new RunSolutionResult
				{
					IsCompileError = true,
					CompilationError = "Слишком большой код."
				});
			}
			var exerciseSlide = courseManager.GetExerciseSlide(courseId, slideIndex);
			var result = await CheckSolution(exerciseSlide, code);
			await SaveUserSolution(courseId, exerciseSlide.Id, code, result.CompilationError, result.ActualOutput, result.IsRightAnswer);
			return Json(result);
		}

		private string NormalizeString(string s)
		{
			return s.LineEndingsToUnixStyle().Trim();
		}

		private async Task<RunSolutionResult> CheckSolution(ExerciseSlide exerciseSlide, string code)
		{
			var solution = exerciseSlide.Solution.BuildSolution(code);
			if (solution.HasErrors)
				return new RunSolutionResult { IsCompileError = true, CompilationError = solution.ErrorMessage };
			if (solution.HasStyleIssues)
				return new RunSolutionResult { IsStyleViolation = true, CompilationError = solution.StyleMessage };
			var submition = await executionService.Submit(solution.SourceCode, "");
			if (submition == null)
				return new RunSolutionResult { IsCompillerFailure = true, CompilationError = "Ой-ой, Sphere Engine, проверяющий задачи, не работает. Попробуйте отправить решение позже." };
			var output = GetOutput(submition);
			var expectedOutput = NormalizeString(exerciseSlide.ExpectedOutput);
			var isRightAnswer = submition.Result == SubmitionResult.Success && output.Equals(expectedOutput);
			var compilationError = submition.Result != SubmitionResult.CompilationError ? "" : submition.CompilationError == "" ? "Compilation Error" : submition.CompilationError;
			return new RunSolutionResult
			{
				IsCompileError = submition.Result == SubmitionResult.CompilationError,
				CompilationError = compilationError,
				IsRightAnswer = isRightAnswer,
				ExpectedOutput = exerciseSlide.HideExpectedOutputOnError ? null : expectedOutput,
				ActualOutput = output
			};
		}

		private string GetOutput(GetSubmitionDetailsResult submition)
		{
			var output = submition.Output;
			if (!string.IsNullOrEmpty(submition.StdErr)) output += "\n" + submition.StdErr;
			if (submition.Result != SubmitionResult.Success)
			{
				if (submition.Time > TimeSpan.FromSeconds(4))
					output += "\n Time limit exceeded";
				else
					output += "\n" + submition.Result;
			}
			return NormalizeString(output);
		}

		private async Task SaveUserSolution(string courseId, string slideId, string code, string compilationError, string output,
			bool isRightAnswer)
		{
			await solutionsRepo.AddUserSolution(
				courseId, slideId,
				code, isRightAnswer, compilationError, output,
				User.Identity.GetUserId());
		}


		private static string GetUserCode(Stream inputStream)
		{
			var codeBytes = new MemoryStream();
			inputStream.CopyTo(codeBytes);
			return Encoding.UTF8.GetString(codeBytes.ToArray());
		}
	}
}