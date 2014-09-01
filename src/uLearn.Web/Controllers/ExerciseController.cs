using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
			var exerciseSlide = courseManager.GetExerciseSlide(courseId, slideIndex);
			var result = await CheckSolution(exerciseSlide, code, slideIndex);
			await SaveUserSolution(courseId, exerciseSlide.Id, code, result.CompilationError, result.ActualOutput, result.IsRightAnswer);
			return Json(result);
		}

		[HttpPost]
		[Authorize]
		public ContentResult PrepareSolution(string courseId, int slideIndex = 0)
		{
			var code = GetUserCode(Request.InputStream);
			var exerciseSlide = courseManager.GetExerciseSlide(courseId, slideIndex);
			var solution = exerciseSlide.Solution.BuildSolution(code);
			return Content(solution.SourceCode ?? solution.ErrorMessage, "text/plain");
		}

		[HttpPost]
		[Authorize]
		public ActionResult FakeRunSolution(string courseId, int slideIndex = 0)
		{
			return Json(new RunSolutionResult
			{
				IsRightAnswer = false,
				ActualOutput = string.Join("\n", Enumerable.Repeat("Тридцать три корабля лавировали лавировали, да не вылавировали", 21)),
				ExpectedOutput = string.Join("\n", Enumerable.Repeat("Тридцать три корабля лавировали лавировали, да не вылавировали", 15)),
			});
		}

		private string NormalizeString(string s)
		{
			return s.LineEndingsToUnixStyle().Trim();
		}

		private async Task<RunSolutionResult> CheckSolution(ExerciseSlide exerciseSlide, string code, int slideIndex)
		{
			var solution = exerciseSlide.Solution.BuildSolution(code);
			if (solution.HasErrors)
				return new RunSolutionResult
				{
					CompilationError = solution.ErrorMessage,
					IsRightAnswer = false,
					ExpectedOutput = "",
					ActualOutput = ""
				};
			var submition = await executionService.Submit(solution.SourceCode, "");
			if (submition == null)
				return new RunSolutionResult
				{
					CompilationError = "Ой-ой, Sphere Engine, проверяющий задачи, не работает. Попробуйте отправить решение позже.",
					IsRightAnswer = false,
					ExpectedOutput = "",
					ActualOutput = ""
				};
			var output = submition.Output;
			if (!string.IsNullOrEmpty(submition.StdErr)) output += "\n" + submition.StdErr;
			if (submition.Result != SubmitionResult.Success)
			{
				if (submition.Time > TimeSpan.FromSeconds(4))
					output += "\n Time limit exceeded";
				else
					output += "\n" + submition.Result;
			}
			output = NormalizeString(output);
			var expectedOutput = NormalizeString(exerciseSlide.ExpectedOutput);
			var isRightAnswer = submition.Result == SubmitionResult.Success && output.Equals(expectedOutput);
			return new RunSolutionResult
			{
				CompilationError = submition.Result != SubmitionResult.CompilationError ? "" : submition.CompilationError == "" ? "Compilation Error" : submition.CompilationError,
				IsRightAnswer = isRightAnswer,
				ExpectedOutput = exerciseSlide.HideExpectedOutputOnError ? null : expectedOutput,
				ActualOutput = output
			};
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