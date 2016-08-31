using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
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

		[System.Web.Mvc.HttpPost]
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
			var submission = await solutionsRepo.RunUserSolution(
				courseId, exerciseSlide.Id, userId, 
				code, null, null, false, "uLearn", 
				GenerateSubmissionName(exerciseSlide), executionTimeout
			);

			if (submission == null)
				return new RunSolutionResult
				{
					IsCompillerFailure = true,
					CompilationError = "Ой-ой, штуковина, которая проверяет решения, сломалась (или просто устала).\nПопробуйте отправить решение позже — когда она немного отдохнет.",
					ExecutionServiceName = "uLearn"
				};

			var automaticChecking = submission.AutomaticChecking;
			var sendToReview = exerciseBlock.RequireReview && automaticChecking.IsRightAnswer;
			if (sendToReview)
				await slideCheckingsRepo.AddManualExerciseChecking(courseId, exerciseSlide.Id, userId, submission);
			await visitsRepo.UpdateScoreForVisit(courseId, exerciseSlide.Id, userId);

			return new RunSolutionResult
			{
				IsCompileError = automaticChecking.IsCompilationError,
				CompilationError = automaticChecking.CompilationError.Text,
				IsRightAnswer = automaticChecking.IsRightAnswer,
				ExpectedOutput = exerciseBlock.HideExpectedOutputOnError ? null : exerciseSlide.Exercise.ExpectedOutput.NormalizeEoln(),
				ActualOutput = automaticChecking.Output.Text,
				ExecutionServiceName = automaticChecking.ExecutionServiceName
			};
		}

		private string GenerateSubmissionName(Slide exerciseSlide)
		{
			return $"{User.Identity.Name}: {exerciseSlide.Info.UnitName} - {exerciseSlide.Title}";
		}

		[System.Web.Mvc.HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public async Task<ActionResult> AddExerciseCodeReview(string courseId, int checkingId, [FromBody] ReviewInfo review)
		{
			var checking = slideCheckingsRepo.FindManualCheckingById<ManualExerciseChecking>(checkingId);
			if (checking.CourseId != courseId)
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			if (!checking.IsLockedBy(User.Identity))
				return RedirectToAction("ManualExerciseCheckingQueue", "Admin", new { courseId, message = "time_is_over" });

			await slideCheckingsRepo.AddExerciseCodeReview(checking, User.Identity.GetUserId(), review.StartLine, review.StartPosition, review.FinishLine, review.FinishPosition, review.Comment);

			return Json(new { status = "ok" });
		}

		[System.Web.Mvc.HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public async Task<ActionResult> DeleteExerciseCodeReview(string courseId, int reviewId)
		{
			var review = slideCheckingsRepo.FindExerciseCodeReviewById(reviewId);
			if (review.AuthorId != User.Identity.GetUserId())
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			await slideCheckingsRepo.DeleteExerciseCodeReview(review);

			return Json(new { status = "ok" });
		}
	}

	public class ReviewInfo
	{
		public string Comment { get; set; }
		public int StartLine { get; set; }
		public int StartPosition { get; set; }
		public int FinishLine { get; set; }
		public int FinishPosition { get; set; }
	}
}

