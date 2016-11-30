using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using log4net;
using Microsoft.AspNet.Identity;
using uLearn.Model.Blocks;
using uLearn.Web.DataContexts;
using uLearn.Web.FilterAttributes;
using uLearn.Web.LTI;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize]
	public class ExerciseController : Controller
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ExerciseController));
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
		public async Task<ActionResult> RunSolution(string courseId, Guid slideId, bool isLti = false)
		{
			/* Check that no checking solution by this user in last 2 minutes */
			var twoMinutesAgo = DateTime.Now.Subtract(TimeSpan.FromMinutes(2));
			if (solutionsRepo.IsCheckingSubmissionByUser(courseId, slideId, User.Identity.GetUserId(), twoMinutesAgo, DateTime.MaxValue))
			{
				return Json(new RunSolutionResult
				{
					Ignored = true,
					ErrorMessage = "Ваше решение по этой задаче уже проверяется. Дождитесь окончания проверки"
				});
			}

			var code = Request.InputStream.GetString();
			if (code.Length > TextsRepo.MaxTextSize)
			{
				return Json(new RunSolutionResult
				{
					IsCompileError = true,
					ErrorMessage = "Слишком большой код"
				});
			}
			var exerciseSlide = courseManager.GetCourse(courseId).FindSlideById(slideId) as ExerciseSlide;
			if (exerciseSlide == null)
				return HttpNotFound();

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
				return new RunSolutionResult { IsCompileError = true, ErrorMessage = solution.ErrorMessage, ExecutionServiceName = "uLearn" };
			if (solution.HasStyleIssues)
				return new RunSolutionResult { IsStyleViolation = true, ErrorMessage = solution.StyleMessage, ExecutionServiceName = "uLearn" };
			var submission = await solutionsRepo.RunUserSolution(
				courseId, exerciseSlide.Id, userId,
				userCode, null, null, false, "uLearn",
				GenerateSubmissionName(exerciseSlide), executionTimeout
				);

			if (submission == null)
			{
				log.Error($"Не смог запустить проверку решения, никто не взял его на проверку.\nКурс: {courseId}, слайд «{exerciseSlide.Title}» ({exerciseSlide.Id})");
				return new RunSolutionResult
				{
					IsCompillerFailure = true,
					ErrorMessage = "Ой-ой, штуковина, которая проверяет решения, сломалась (или просто устала).\nПопробуйте отправить решение позже — когда она немного отдохнет.",
					ExecutionServiceName = "uLearn"
				};
			}

			var automaticChecking = submission.AutomaticChecking;
			var isProhibitedUserToSendForReview = slideCheckingsRepo.IsProhibitedToSendExerciseToManualChecking(courseId, exerciseSlide.Id, userId);
			var sendToReview = exerciseBlock.RequireReview && automaticChecking.IsRightAnswer && !isProhibitedUserToSendForReview;
			if (sendToReview)
			{
				await slideCheckingsRepo.RemoveWaitingManualExerciseCheckings(courseId, exerciseSlide.Id, userId);
				await slideCheckingsRepo.AddManualExerciseChecking(courseId, exerciseSlide.Id, userId, submission);
			}
			await visitsRepo.UpdateScoreForVisit(courseId, exerciseSlide.Id, userId);

			return new RunSolutionResult
			{
				IsCompileError = automaticChecking.IsCompilationError,
				ErrorMessage = automaticChecking.CompilationError.Text,
				IsRightAnswer = automaticChecking.IsRightAnswer,
				ExpectedOutput = exerciseBlock.HideExpectedOutputOnError ? null : exerciseSlide.Exercise.ExpectedOutput.NormalizeEoln(),
				ActualOutput = automaticChecking.Output.Text,
				ExecutionServiceName = automaticChecking.ExecutionServiceName,
				SentToReview = sendToReview,
				SubmissionId = submission.Id,
			};
		}

		private string GenerateSubmissionName(Slide exerciseSlide)
		{
			return $"{User.Identity.Name}: {exerciseSlide.Info.UnitName} - {exerciseSlide.Title}";
		}

		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		[System.Web.Mvc.HttpPost]
		[ValidateInput(false)]
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

			return PartialView("_ExerciseReview", new ExerciseCodeReviewModel
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

		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		[System.Web.Mvc.HttpPost]
		[ValidateInput(false)]
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
		public async Task<ActionResult> ScoreExercise(int id, string nextUrl, string exerciseScore, bool? prohibitFurtherReview, string errorUrl = "", bool recheck = false)
		{
			if (string.IsNullOrEmpty(errorUrl))
				errorUrl = nextUrl;

			using (var transaction = db.Database.BeginTransaction())
			{
				var checking = slideCheckingsRepo.FindManualCheckingById<ManualExerciseChecking>(id);

				if (checking.IsChecked && !recheck)
					return Redirect(errorUrl + "Эта работа уже была проверена");

				if (!checking.IsLockedBy(User.Identity))
					return Redirect(errorUrl + "Эта работа проверяется другим инструктором");

				var course = courseManager.GetCourse(checking.CourseId);
				var slide = (ExerciseSlide)course.GetSlideById(checking.SlideId);
				var exercise = slide.Exercise;

				int score;
				/* Invalid form: score isn't integer */
				if (!int.TryParse(exerciseScore, out score))
					return Redirect(errorUrl + "Неверное количество баллов");

				/* Invalid form: score isn't from range 0..MAX_SCORE */
				if (score < 0 || score > exercise.MaxReviewScore)
					return Redirect(errorUrl + $"Неверное количество баллов: {score}");

				checking.ProhibitFurtherManualCheckings = false;
				await slideCheckingsRepo.MarkManualCheckingAsChecked(checking, score);
				if (prohibitFurtherReview.HasValue && prohibitFurtherReview.Value)
					await slideCheckingsRepo.ProhibitFurtherExerciseManualChecking(checking);
				await visitsRepo.UpdateScoreForVisit(checking.CourseId, checking.SlideId, checking.UserId);

				transaction.Commit();
			}

			return Redirect(nextUrl);
		}

		[System.Web.Mvc.HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public async Task<ActionResult> SimpleScoreExercise(int submissionId, int exerciseScore, bool ignoreNewestSubmission=false, int? updateCheckingId = null)
		{
			var submission = solutionsRepo.FindSubmissionById(submissionId);
			var courseId = submission.CourseId;
			var slideId = submission.SlideId;
			var userId = submission.UserId;

			if (!User.HasAccessFor(courseId, CourseRole.Instructor))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			if (!ignoreNewestSubmission && !updateCheckingId.HasValue)
			{
				var lastAcceptedSubmission = solutionsRepo.GetAllAcceptedSubmissionsByUser(courseId, slideId, userId).OrderByDescending(s => s.Timestamp).FirstOrDefault();
				if (lastAcceptedSubmission != null && lastAcceptedSubmission.Id != submission.Id)
					return Json(
						new {
							status = "error",
							error = "has_newest_submission",
							submissionId = lastAcceptedSubmission.Id,
							submissionDate = lastAcceptedSubmission.Timestamp.ToAgoPrettyString(true)
						});
			}

			var manualScore = slideCheckingsRepo.GetManualScoreForSlide(courseId, slideId, userId);
			if (exerciseScore < manualScore && !updateCheckingId.HasValue)
				return Json(
					new
					{
						status = "error",
						error = "has_greatest_score",
						score = manualScore,
						checkedQueueUrl = Url.Action("ManualExerciseCheckingQueue", "Admin", new { courseId, done = true, userId, slideId})
					});

			/* TODO: check if 0 <= exercisScore <= exercise.MaxReviewScore */

			await slideCheckingsRepo.RemoveWaitingManualExerciseCheckings(courseId, slideId, userId);
			ManualExerciseChecking checking;
			if (updateCheckingId.HasValue)
				checking = slideCheckingsRepo.FindManualCheckingById<ManualExerciseChecking>(updateCheckingId.Value);
			else
				checking = await slideCheckingsRepo.AddManualExerciseChecking(courseId, slideId, userId, submission);
			await slideCheckingsRepo.LockManualChecking(checking, User.Identity.GetUserId());
			await slideCheckingsRepo.MarkManualCheckingAsChecked(checking, exerciseScore);
			await visitsRepo.UpdateScoreForVisit(courseId, slideId, userId);

			return Json(
				new
				{
					status = "ok",
					score = exerciseScore.PluralizeInRussian(new RussianPluralizationOptions { One = "балл", Two = "балла", Five = "баллов", Gender = Gender.Male, hideNumberOne = false, smallNumbersAreWords = false}),
					totalScore = visitsRepo.GetScore(slideId, userId),
					checkingId = checking.Id,
				});
		}

		public ActionResult SubmissionsPanel(string courseId, Guid slideId, string userId = "", int? currentSubmissionId = null, bool canTryAgain=true)
		{
			if (!User.HasAccessFor(courseId, CourseRole.Instructor))
				userId = "";

			if (userId == "")
				userId = User.Identity.GetUserId();

			var slide = courseManager.GetCourse(courseId).FindSlideById(slideId);
			var submissions = solutionsRepo.GetAllAcceptedSubmissionsByUser(courseId, slideId, userId).ToList();

			return PartialView(new ExerciseSubmissionsPanelModel(courseId, slide)
			{
				Submissions = submissions,
				CurrentSubmissionId = currentSubmissionId,
				CanTryAgain = canTryAgain,
			});
		}

		private ExerciseBlockData CreateExerciseBlockData(Course course, Slide slide, UserExerciseSubmission submission, bool onlyAccepted, string currentUserId)
		{
			var userId = submission?.UserId ?? currentUserId;
			var visit = visitsRepo.FindVisiter(course.Id, slide.Id, userId);

			var solution = submission?.SolutionCode.Text;
			var submissionReviews = submission?.ManualCheckings.LastOrDefault()?.NotDeletedReviews;

			var hasUncheckedReview = submission?.ManualCheckings.Any(c => !c.IsChecked) ?? false;
			var hasCheckedReview = submission?.ManualCheckings.Any(c => c.IsChecked) ?? false;
			var reviewState = hasCheckedReview ? ExerciseReviewState.Reviewed :
				hasUncheckedReview ? ExerciseReviewState.WaitingForReview :
					ExerciseReviewState.NotReviewed;

			var submissions = onlyAccepted ?
				solutionsRepo.GetAllAcceptedSubmissionsByUser(course.Id, slide.Id, userId) :
				solutionsRepo.GetAllSubmissionsByUser(course.Id, slide.Id, userId);
			var topUserReviewComments = slideCheckingsRepo.GetTopUserReviewComments(course.Id, slide.Id, currentUserId, 10);

			return new ExerciseBlockData(course.Id, (ExerciseSlide) slide, visit?.IsSkipped ?? false, solution)
			{
				Url = Url,
				Reviews = submissionReviews?.ToList() ?? new List<ExerciseCodeReview>(),
				ReviewState = reviewState,
				IsGuest = false,
				SubmissionSelectedByUser = submission,
				Submissions = submissions.ToList(),
				TopUserReviewComments = topUserReviewComments,
			};
		}

		private UserExerciseSubmission GetExerciseSubmissionShownByDefault(string courseId, Guid slideId, string userId)
		{
			var submissions = solutionsRepo.GetAllAcceptedSubmissionsByUser(courseId, slideId, userId).ToList();
			return submissions.LastOrDefault(s => s.ManualCheckings != null && s.ManualCheckings.Any()) ??
					submissions.LastOrDefault(s => s.AutomaticCheckingIsRightAnswer);
		}

		public ActionResult Submission(string courseId, Guid slideId, string userId=null, int? submissionId=null, int? manualCheckingId = null, bool isLti = false, bool showOutput = false, bool instructorView = false, bool onlyAccepted = true)
		{
			if (!User.HasAccessFor(courseId, CourseRole.Instructor))
				instructorView = false;
			
			var currentUserId = userId ?? User.Identity.GetUserId();
			UserExerciseSubmission submission = null;
			if (submissionId.HasValue && submissionId.Value > 0)
			{
				submission = solutionsRepo.FindSubmissionById(submissionId.Value);
				if (submission == null)
					return HttpNotFound();
				if (courseId != submission.CourseId)
					return HttpNotFound();
				if (slideId != submission.SlideId)
					return HttpNotFound();
				if (!User.HasAccessFor(courseId, CourseRole.Instructor) && submission.UserId != currentUserId)
					return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			}
			else if (! submissionId.HasValue && ! manualCheckingId.HasValue)
			{
				submission = GetExerciseSubmissionShownByDefault(courseId, slideId, currentUserId);
			}

			var course = courseManager.GetCourse(courseId);
			var slide = course.FindSlideById(slideId);
			if (slide == null)
				return HttpNotFound();

			ManualExerciseChecking manualChecking = null;
			if (User.HasAccessFor(courseId, CourseRole.Instructor) && manualCheckingId.HasValue)
			{
				manualChecking = slideCheckingsRepo.FindManualCheckingById<ManualExerciseChecking>(manualCheckingId.Value);
			}
			if (manualChecking != null && !submissionId.HasValue)
				submission = manualChecking.Submission;

			var model = CreateExerciseBlockData(course, slide, submission, onlyAccepted, currentUserId);
			model.IsLti = isLti;
			model.ShowOutputImmediately = showOutput;
			model.InstructorView = instructorView;
			model.ShowOnlyAccepted = onlyAccepted;
			if (manualChecking != null)
			{
				if (manualChecking.CourseId == courseId)
				{
					model.ManualChecking = manualChecking;
					model.Reviews = manualChecking.NotDeletedReviews;
				}
			}

			return PartialView(model);
		}
	}

	public class ReviewInfo
	{
		[AllowHtml]
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
			CanTryAgain = true;
		}

		public string CourseId { get; set; }
		public Slide Slide { get; set; }
		public List<UserExerciseSubmission> Submissions { get; set; }
		public int? CurrentSubmissionId { get; set; }
		public bool CanTryAgain { get; set; }
	}

	public class ExerciseControlsModel
	{
		public ExerciseControlsModel(string courseId, ExerciseSlide slide)
		{
			CourseId = courseId;
			Slide = slide;
		}

		public string CourseId;
		public ExerciseSlide Slide;
		public ExerciseBlock Block => Slide.Exercise;

		public bool IsLti = false;
		public bool IsCodeEditableAndSendable = true;
		public bool DebugView = false;
		public bool CanShowOutput = false;
		public bool IsShowOutputButtonActive = false;
		public string AcceptedSolutionsAction = "";
		public string RunSolutionUrl = "";
		public string UseHintUrl = "";

		public bool HideShowSolutionsButton => Block.HideShowSolutionsButton;
	}

	public class ExerciseScoreFormModel
	{
		public ExerciseScoreFormModel(string courseId, ExerciseSlide slide, ManualExerciseChecking checking, int? groupId = null, bool isCurrentSubmissionChecking = false)
		{
			CourseId = courseId;
			Slide = slide;
			Checking = checking;
			GroupId = groupId;
			IsCurrentSubmissionChecking = isCurrentSubmissionChecking;
		}

		public string CourseId { get; set; }
		public ExerciseSlide Slide { get; set; }
		public ManualExerciseChecking Checking { get; set; }
		public int? GroupId { get; set; }
		public bool IsCurrentSubmissionChecking { get; set; }
	}
}

