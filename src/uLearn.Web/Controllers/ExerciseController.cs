using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CsSandboxApi;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class ExerciseController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly CsSandboxClient executionService = new CsSandboxClient();

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
			var submissionDetails = await executionService.Submit(solution.SourceCode, "");
			if (submissionDetails == null)
				return new RunSolutionResult { IsCompillerFailure = true, CompilationError = "Ой-ой, CsSandbox, проверяющий задачи, не работает. Попробуйте отправить решение позже." };
			var output = GetOutput(submissionDetails);
			var expectedOutput = NormalizeString(exerciseSlide.ExpectedOutput);
			var isRightAnswer = submissionDetails.IsSuccess() && output.Equals(expectedOutput);
			return new RunSolutionResult
			{
				IsCompileError = submissionDetails.IsCompilationError(),
				CompilationError = submissionDetails.GetCompilationError(),
				IsRightAnswer = isRightAnswer,
				ExpectedOutput = exerciseSlide.HideExpectedOutputOnError ? null : expectedOutput,
				ActualOutput = output
			};
		}

		private string GetOutput(PublicSubmissionDetails submition)
		{
			var output = submition.Output;
			if (!string.IsNullOrEmpty(submition.Error)) output += "\n" + submition.Error;
			if (!submition.IsSuccess())
			{
				if (submition.IsTimeLimit())
					output += "\n Time limit exceeded";
				else
					output += "\n" + submition.Verdict;
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