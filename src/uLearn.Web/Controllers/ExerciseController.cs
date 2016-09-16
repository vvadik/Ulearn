using System;
using System.Collections.Generic;
using System.Linq;
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
		private readonly ULearnDb db = new ULearnDb();
		private readonly UsersRepo usersRepo = new UsersRepo();
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


		private async Task<RunSolutionResult> CheckSolution(string courseId, ExerciseSlide exerciseSlide, string userCode)
		{
			var exerciseBlock = exerciseSlide.Exercise;
			var userId = User.Identity.GetUserId();
		    var solution = exerciseBlock.BuildSolution(userCode);
		    if (solution.HasErrors)
		        return new RunSolutionResult { IsCompileError = true, CompilationError = solution.ErrorMessage, ExecutionServiceName = "uLearn" };
		    if (solution.HasStyleIssues)
		        return new RunSolutionResult { IsStyleViolation = true, CompilationError = solution.StyleMessage, ExecutionServiceName = "uLearn" };
		    var submission = await solutionsRepo.RunUserSolution(
				courseId, exerciseSlide.Id, userId, 
				userCode, null, null, false, "uLearn", 
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
			var isProhibitedUserToSendForReview = slideCheckingsRepo.IsProhibitedToSendExerciseToManualChecking(courseId, exerciseSlide.Id, userId);
			var sendToReview = exerciseBlock.RequireReview && automaticChecking.IsRightAnswer && ! isProhibitedUserToSendForReview;
			if (sendToReview)
			{
				await slideCheckingsRepo.RemoveWaitingManualExerciseCheckings(courseId, exerciseSlide.Id, userId);
				await slideCheckingsRepo.AddManualExerciseChecking(courseId, exerciseSlide.Id, userId, submission);
			}
			await visitsRepo.UpdateScoreForVisit(courseId, exerciseSlide.Id, userId);

			return new RunSolutionResult
			{
				IsCompileError = automaticChecking.IsCompilationError,
				CompilationError = automaticChecking.CompilationError.Text,
				IsRightAnswer = automaticChecking.IsRightAnswer,
				ExpectedOutput = exerciseBlock.HideExpectedOutputOnError ? null : exerciseSlide.Exercise.ExpectedOutput.NormalizeEoln(),
				ActualOutput = automaticChecking.Output.Text,
				ExecutionServiceName = automaticChecking.ExecutionServiceName,
				SentToReview = sendToReview,
			};
		}

		private string GenerateSubmissionName(Slide exerciseSlide)
		{
			return $"{User.Identity.Name}: {exerciseSlide.Info.UnitName} - {exerciseSlide.Title}";
		}

		[System.Web.Mvc.HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public async Task<ActionResult> AddExerciseCodeReview(string courseId, int checkingId, [FromBody] ReviewInfo reviewInfo)
		{
			var checking = slideCheckingsRepo.FindManualCheckingById<ManualExerciseChecking>(checkingId);
			if (checking.CourseId != courseId)
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			if (!checking.IsLockedBy(User.Identity))
				return RedirectToAction("ManualExerciseCheckingQueue", "Admin", new { courseId, message = "time_is_over" });

			/* Make start position less than finish position */
			if (reviewInfo.StartLine > reviewInfo.FinishLine || (reviewInfo.StartLine == reviewInfo.FinishLine && reviewInfo.StartPosition > reviewInfo.FinishPosition))
			{
				var tmp = reviewInfo.StartLine;
				reviewInfo.StartLine = reviewInfo.FinishLine;
				reviewInfo.FinishLine = tmp;

				tmp = reviewInfo.StartPosition;
				reviewInfo.StartPosition = reviewInfo.FinishPosition;
				reviewInfo.FinishPosition = tmp;
			}
			
			var review = await slideCheckingsRepo.AddExerciseCodeReview(checking, User.Identity.GetUserId(), reviewInfo.StartLine, reviewInfo.StartPosition, reviewInfo.FinishLine, reviewInfo.FinishPosition, reviewInfo.Comment);

			return PartialView("~/Views/Course/_ExerciseReview.cshtml", new ExerciseCodeReviewModel
			{
				Review = review,
				ManualChecking = checking,
			});
		}

		[System.Web.Mvc.HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public async Task<ActionResult> DeleteExerciseCodeReview(string courseId, int reviewId)
		{
			var review = slideCheckingsRepo.FindExerciseCodeReviewById(reviewId);
			if (review.ExerciseChecking.CourseId != courseId)
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			if (review.AuthorId != User.Identity.GetUserId())
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			await slideCheckingsRepo.DeleteExerciseCodeReview(review);

			return Json(new { status = "ok" });
		}

		[System.Web.Mvc.HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public async Task<ActionResult> UpdateExerciseCodeReview(string courseId, int reviewId, string comment)
		{
			var review = slideCheckingsRepo.FindExerciseCodeReviewById(reviewId);
			if (review.ExerciseChecking.CourseId != courseId)
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			if (review.AuthorId != User.Identity.GetUserId())
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			await slideCheckingsRepo.UpdateExerciseCodeReview(review, comment);

			return Json(new { status = "ok" });
		}

		[System.Web.Mvc.HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public async Task<ActionResult> ScoreExercise(int id, string nextUrl, int exerciseScore, bool? prohibitFurtherReview, string errorUrl = "", bool recheck = false)
		{
			if (string.IsNullOrEmpty(errorUrl))
				errorUrl = nextUrl;

			using (var transaction = db.Database.BeginTransaction())
			{
				var checking = slideCheckingsRepo.FindManualCheckingById<ManualExerciseChecking>(id);

				if (checking.IsChecked && ! recheck)
					return Redirect(errorUrl + "Эта работа уже была проверена");

				if (!checking.IsLockedBy(User.Identity))
					return Redirect(errorUrl + "Эта работа проверяется другим инструктором");

				var course = courseManager.GetCourse(checking.CourseId);
				var slide = (ExerciseSlide) course.GetSlideById(checking.SlideId);
				var exercise = slide.Exercise;

				/* Invalid form: score isn't from range 0..MAX_SCORE */
				if (exerciseScore < 0 || exerciseScore > exercise.MaxReviewScore)
					return Redirect(errorUrl + $"Неверное количество баллов: {exerciseScore}");

				checking.ProhibitFurtherManualCheckings = false;
				await slideCheckingsRepo.MarkManualCheckingAsChecked(checking, exerciseScore);
				if (prohibitFurtherReview.HasValue && prohibitFurtherReview.Value)
					await slideCheckingsRepo.ProhibitFurtherExerciseManualChecking(checking);
				await visitsRepo.UpdateScoreForVisit(checking.CourseId, checking.SlideId, checking.UserId);

				transaction.Commit();
			}

			return Redirect(nextUrl);
		}

		public ActionResult SubmissionsPanel(string courseId, Guid slideId, string userId="")
		{
			if (!User.HasAccessFor(courseId, CourseRole.Instructor))
				userId = "";

			if (userId == "")
				userId = User.Identity.GetUserId();

			var slide = courseManager.GetCourse(courseId).FindSlideById(slideId);
			var submissions = solutionsRepo.GetAllAcceptedSubmissionsByUser(courseId, slideId, userId).ToList();

			return PartialView("~/Views/Course/_ExerciseSubmissionsPanel.cshtml", new ExerciseSubmissionsPanelModel(courseId, slide)
			{
				Submissions = submissions,
			});
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

	public class ExerciseSubmissionsPanelModel
	{
		public ExerciseSubmissionsPanelModel(string courseId, Slide slide)
		{
			CourseId = courseId;
			Slide = slide;

			Submissions = new List<UserExerciseSubmission>();
			CurrentSubmissionId = null;
		}

		public string CourseId { get; set; }
		public Slide Slide { get; set; }
		public List<UserExerciseSubmission> Submissions { get; set; }
		public int? CurrentSubmissionId { get; set; }
	}
}

