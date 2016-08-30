using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.FilterAttributes;
using uLearn.Web.LTI;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize]
	public class ExerciseController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly VisitsRepo visitsRepo = new VisitsRepo();
		private readonly SlideCheckingsRepo slideCheckingsRepo = new SlideCheckingsRepo();

		private static readonly TimeSpan executionTimeout = TimeSpan.FromSeconds(30);

		public ExerciseController()
			: this(WebCourseManager.Instance)
		{
		}

		public ExerciseController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[HttpPost]
		public async Task<ActionResult> RunSolution(string courseId, int slideIndex = 0, bool isLti = false)
		{
			var code = Request.InputStream.GetString();
			if (code.Length > TextsRepo.MaxTextSize)
			{
				return Json(new RunSolutionResult
				{
					IsCompileError = true,
					CompilationError = "Слишком большой код."
				});
			}
			var exerciseSlide = (ExerciseSlide)courseManager.GetCourse(courseId).Slides[slideIndex];
			
			var result = await CheckSolution(courseId, exerciseSlide, code);
			if (isLti)
				LtiUtils.SubmitScore(exerciseSlide, User.Identity.GetUserId());

			return Json(result);
		}


		private async Task<RunSolutionResult> CheckSolution(string courseId, ExerciseSlide exerciseSlide, string code)
		{
			var exerciseBlock = exerciseSlide.Exercise;
		    var solution = exerciseBlock.BuildSolution(code);
			var userId = User.Identity.GetUserId();
			if (solution.HasErrors)
		        return new RunSolutionResult { IsCompileError = true, CompilationError = solution.ErrorMessage, ExecutionServiceName = "uLearn"};
		    if (solution.HasStyleIssues)
		        return new RunSolutionResult { IsStyleViolation = true, CompilationError = solution.StyleMessage, ExecutionServiceName = "uLearn" };
			var automaticExerciseChecking = await solutionsRepo.RunUserSolution(
				courseId, exerciseSlide.Id, userId, 
				code, null, null, false, "uLearn", 
				GenerateSubmissionName(exerciseSlide), executionTimeout
			);

			if (automaticExerciseChecking == null)
				return new RunSolutionResult
				{
					IsCompillerFailure = true,
					CompilationError = "Ой-ой, штуковина, которая проверяет решения, сломалась (или просто устала).\nПопробуйте отправить решение позже — когда она немного отдохнет.",
					ExecutionServiceName = "uLearn"
				};
			
			var sendToReview = exerciseBlock.RequireReview && automaticExerciseChecking.IsRightAnswer;
			if (sendToReview)
				await slideCheckingsRepo.AddManualExerciseChecking(courseId, exerciseSlide.Id, userId);
			await visitsRepo.UpdateScoreForVisit(courseId, exerciseSlide.Id, userId);

			return new RunSolutionResult
			{
				IsCompileError = automaticExerciseChecking.IsCompilationError,
				CompilationError = automaticExerciseChecking.CompilationError.Text,
				IsRightAnswer = automaticExerciseChecking.IsRightAnswer,
				ExpectedOutput = exerciseBlock.HideExpectedOutputOnError ? null : exerciseSlide.Exercise.ExpectedOutput.NormalizeEoln(),
				ActualOutput = automaticExerciseChecking.Output.Text,
				ExecutionServiceName = automaticExerciseChecking.ExecutionServiceName
			};
		}

		private string GenerateSubmissionName(Slide exerciseSlide)
		{
			return string.Format("{0}: {1} - {2}", User.Identity.Name, exerciseSlide.Info.UnitName, exerciseSlide.Title);
		}
	}
}
