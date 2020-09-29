using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.CourseRoles;
using Database.Repos.Groups;
using Database.Repos.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Exercises;
using Ulearn.Core.Courses.Slides.Exercises.Blocks;
using Ulearn.Core.CSharp;
using Ulearn.Core.Metrics;
using Ulearn.Core.Telegram;
using Ulearn.Web.Api.Models.Parameters.Exercise;
using Ulearn.Web.Api.Models.Responses.Exercise;
using Ulearn.Web.Api.Utils.LTI;

namespace Ulearn.Web.Api.Controllers.Slides
{
	public class ExerciseController : BaseController
	{
		private readonly IUserSolutionsRepo userSolutionsRepo;
		private readonly ICourseRolesRepo courseRolesRepo;
		private readonly IVisitsRepo visitsRepo;
		private readonly ISlideCheckingsRepo slideCheckingsRepo;
		private readonly IGroupsRepo groupsRepo;
		private readonly ILtiConsumersRepo ltiConsumersRepo;
		private readonly ILtiRequestsRepo ltiRequestsRepo;
		private readonly IStyleErrorsRepo styleErrorsRepo;
		private readonly MetricSender metricSender;
		private readonly ErrorsBot errorsBot = new ErrorsBot();

		public ExerciseController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, MetricSender metricSender,
			IUsersRepo usersRepo, IUserSolutionsRepo userSolutionsRepo, ICourseRolesRepo courseRolesRepo, IVisitsRepo visitsRepo,
			ILtiConsumersRepo ltiConsumersRepo, ILtiRequestsRepo ltiRequestsRepo, ISlideCheckingsRepo slideCheckingsRepo, IGroupsRepo groupsRepo,
			IStyleErrorsRepo styleErrorsRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.metricSender = metricSender;
			this.userSolutionsRepo = userSolutionsRepo;
			this.courseRolesRepo = courseRolesRepo;
			this.visitsRepo = visitsRepo;
			this.ltiConsumersRepo = ltiConsumersRepo;
			this.ltiRequestsRepo = ltiRequestsRepo;
			this.slideCheckingsRepo = slideCheckingsRepo;
			this.groupsRepo = groupsRepo;
			this.styleErrorsRepo = styleErrorsRepo;
		}

		[HttpPost("/slides/{courseId}/{slideId}/exercise/submit")]
		[Authorize]
		public async Task<ActionResult<RunSolutionResponse>> RunSolution(
			[FromRoute] Course course, 
			[FromRoute] Guid slideId,
			[FromBody] RunSolutionParameters parameters,
			[FromQuery] bool isLti = false)
		{
			var courseId = course.Id;

			/* Check that no checking solution by this user in last time */
			var halfMinuteAgo = DateTime.Now.Subtract(TimeSpan.FromSeconds(30));
			if (await userSolutionsRepo.IsCheckingSubmissionByUser(courseId, slideId, User.Identity.GetUserId(), halfMinuteAgo, DateTime.MaxValue))
			{
				return Json(new RunSolutionResponse
				{
					Ignored = true,
					Message = "Ваше решение по этой задаче уже проверяется. Дождитесь окончания проверки"
				});
			}

			var code = parameters.Solution;
			if (code.Length > TextsRepo.MaxTextSize)
			{
				return Json(new RunSolutionResponse
				{
					IsCompileError = true,
					Message = "Слишком большой код"
				});
			}

			var isInstructor = await courseRolesRepo.HasUserAccessToCourseAsync(UserId, courseId, CourseRoleType.Instructor);
			var exerciseSlide = (await courseManager.FindCourseAsync(courseId))?.FindSlideById(slideId, isInstructor) as ExerciseSlide;
			if (exerciseSlide == null)
				return NotFound(new { status = "error", message = "Slide not found" });

			var result = await CheckSolution(
				courseId, exerciseSlide, code, User.Identity.GetUserId(), User.Identity.Name,
				waitUntilChecked: true, saveSubmissionOnCompileErrors: false
			).ConfigureAwait(false);

			if (isLti)
			{
				try
				{
					var score = await visitsRepo.GetScore(courseId, slideId, UserId);
					await LtiUtils.SubmitScore(courseId, exerciseSlide, User.Identity.GetUserId(), score, ltiRequestsRepo, ltiConsumersRepo);
				}
				catch (Exception e)
				{
					logger.Error("Мы не смогли отправить баллы на вашу образовательную платформу", e);
					return Json(new RunSolutionResponse
					{
						IsInternalServerError = true,
						Message = "Мы не смогли отправить баллы на вашу образовательную платформу. Пожалуйста, обновите страницу — мы попробуем сделать это ещё раз."
					});
				}
			}

			return result;
		}

		private async Task<RunSolutionResponse> CheckSolution(string courseId, ExerciseSlide exerciseSlide, string userCode, string userId, string userName, bool waitUntilChecked, bool saveSubmissionOnCompileErrors)
		{
			var exerciseMetricId = GetExerciseMetricId(courseId, exerciseSlide);
			metricSender.SendCount("exercise.try");
			metricSender.SendCount($"exercise.{courseId.ToLower(CultureInfo.InvariantCulture)}.try");
			metricSender.SendCount($"exercise.{exerciseMetricId}.try");

			var course = await courseManager.GetCourseAsync(courseId);
			var exerciseBlock = exerciseSlide.Exercise;
			var buildResult = exerciseBlock.BuildSolution(userCode);

			if (buildResult.HasErrors)
				metricSender.SendCount($"exercise.{exerciseMetricId}.CompilationError");
			if (buildResult.HasStyleErrors)
				metricSender.SendCount($"exercise.{exerciseMetricId}.StyleViolation");
			
			if (!saveSubmissionOnCompileErrors)
			{
				if (buildResult.HasErrors)
					return new RunSolutionResponse { IsCompileError = true, Message = buildResult.ErrorMessage, ExecutionServiceName = "uLearn" };
			}

			var compilationErrorMessage = buildResult.HasErrors ? buildResult.ErrorMessage : null;
			var dontRunSubmission = buildResult.HasErrors;
			var submissionLanguage = exerciseBlock.Language.Value;
			var submissionSandbox = (exerciseBlock as UniversalExerciseBlock)?.DockerImageName;
			var submission = await userSolutionsRepo.AddUserExerciseSubmission(
				courseId, exerciseSlide.Id,
				userCode, compilationErrorMessage, null,
				userId, "uLearn", GenerateSubmissionName(exerciseSlide, userName),
				submissionLanguage,
				submissionSandbox,
				dontRunSubmission ? AutomaticExerciseCheckingStatus.Done : AutomaticExerciseCheckingStatus.Waiting
			);

			if (buildResult.HasErrors)
				return new RunSolutionResponse { IsCompileError = true, Message = buildResult.ErrorMessage, SubmissionId = submission.Id, ExecutionServiceName = "uLearn" };

			var hasAutomaticChecking = submissionLanguage.HasAutomaticChecking() && (submissionLanguage == Language.CSharp || exerciseBlock is UniversalExerciseBlock);
			var executionTimeout = TimeSpan.FromSeconds(exerciseBlock.TimeLimit * 2 + 5);
			try
			{
				if (hasAutomaticChecking)
					await userSolutionsRepo.RunAutomaticChecking(submission, executionTimeout, waitUntilChecked);
			}
			catch (SubmissionCheckingTimeout)
			{
				logger.Error($"Не смог запустить проверку решения, никто не взял его на проверку за {executionTimeout.TotalSeconds} секунд.\nКурс «{course.Title}», слайд «{exerciseSlide.Title}» ({exerciseSlide.Id})");
				await errorsBot.PostToChannelAsync($"Не смог запустить проверку решения, никто не взял его на проверку за {executionTimeout.TotalSeconds} секунд.\nКурс «{course.Title}», слайд «{exerciseSlide.Title}» ({exerciseSlide.Id})\n\nhttps://ulearn.me/Sandbox");
				return new RunSolutionResponse
				{
					IsInternalServerError = true,
					Message = "К сожалению, из-за большой нагрузки мы не смогли оперативно проверить ваше решение. " +
								"Мы попробуем проверить его позже, просто подождите и обновите страницу. ",
					ExecutionServiceName = "ulearn"
				};
			}

			if (!waitUntilChecked)
			{
				metricSender.SendCount($"exercise.{exerciseMetricId}.dont_wait_result");
				return new RunSolutionResponse { SubmissionId = submission.Id };
			}

			/* Update the submission */
			submission = await userSolutionsRepo.FindNoTrackingSubmission(submission.Id);

			if (submission.AutomaticCheckingIsRightAnswer)
				await CreateStyleErrorsReviewsForSubmission(submission, buildResult.StyleErrors, exerciseMetricId);

			var automaticChecking = submission.AutomaticChecking;
			bool sentToReview;
			if (!hasAutomaticChecking)
				sentToReview = await SendToReviewAndUpdateScore(submission, courseManager, slideCheckingsRepo, groupsRepo, visitsRepo, metricSender);
			else
				sentToReview = await slideCheckingsRepo.HasManualExerciseChecking(courseId, exerciseSlide.Id, userId, submission.Id);

			var result = new RunSolutionResponse
			{
				IsCompileError = automaticChecking?.IsCompilationError ?? false,
				Message = automaticChecking?.CompilationError.Text ?? "",
				IsRightAnswer = submission.AutomaticCheckingIsRightAnswer,
				ExpectedOutput = exerciseBlock.HideExpectedOutputOnError ? null : exerciseSlide.Exercise.ExpectedOutput?.NormalizeEoln(),
				ActualOutput = automaticChecking?.Output.Text ?? "",
				ExecutionServiceName = automaticChecking?.ExecutionServiceName ?? "ulearn",
				SentToReview = sentToReview,
				SubmissionId = submission.Id,
			};
			if (buildResult.HasStyleErrors)
			{
				result.StyleMessages = buildResult.StyleErrors.Select(e => e.GetMessageWithPositions()).ToList();
			}

			return result;
		}

		private string GenerateSubmissionName(Slide exerciseSlide, string userName)
		{
			return $"{userName}: {exerciseSlide.Info.Unit.Title} - {exerciseSlide.Title}";
		}
		
		public static async Task<bool> SendToReviewAndUpdateScore(UserExerciseSubmission submission,
			IWebCourseManager courseManager, ISlideCheckingsRepo slideCheckingsRepo, IGroupsRepo groupsRepo, IVisitsRepo visitsRepo, MetricSender metricSender)
		{
			var userId = submission.User.Id;
			var courseId = submission.CourseId;
			var course = await courseManager.GetCourseAsync(courseId);
			var exerciseSlide = course.FindSlideById(submission.SlideId, true) as ExerciseSlide; // SlideId проверен в вызывающем методе 
			if (exerciseSlide == null)
				return false;
			var exerciseMetricId = GetExerciseMetricId(courseId, exerciseSlide);
			var automaticChecking = submission.AutomaticChecking;
			var isProhibitedUserToSendForReview = await slideCheckingsRepo.IsProhibitedToSendExerciseToManualChecking(courseId, exerciseSlide.Id, userId);
			var sendToReview = exerciseSlide.Scoring.RequireReview
								&& submission.AutomaticCheckingIsRightAnswer
								&& !isProhibitedUserToSendForReview
								&& await groupsRepo.IsManualCheckingEnabledForUserAsync(course, userId);
			if (sendToReview)
			{
				await slideCheckingsRepo.RemoveWaitingManualCheckings<ManualExerciseChecking>(courseId, exerciseSlide.Id, userId, false);
				await slideCheckingsRepo.AddManualExerciseChecking(courseId, exerciseSlide.Id, userId, submission.Id);
				await visitsRepo.MarkVisitsAsWithManualChecking(courseId, exerciseSlide.Id, userId);
				metricSender.SendCount($"exercise.{exerciseMetricId}.sent_to_review");
				metricSender.SendCount("exercise.sent_to_review");
			}

			await visitsRepo.UpdateScoreForVisit(courseId, exerciseSlide.Id, exerciseSlide.MaxScore, userId);

			if (automaticChecking != null)
			{
				var verdictForMetric = automaticChecking.GetVerdict().Replace(" ", "");
				metricSender.SendCount($"exercise.{exerciseMetricId}.{verdictForMetric}");
			}

			return sendToReview;
		}

		public static string GetExerciseMetricId(string courseId, ExerciseSlide exerciseSlide)
		{
			var slideTitleForMetric = exerciseSlide.LatinTitle.Replace(".", "_").ToLower(CultureInfo.InvariantCulture);
			if (slideTitleForMetric.Length > 25)
				slideTitleForMetric = slideTitleForMetric.Substring(0, 25);
			return $"{courseId.ToLower(CultureInfo.InvariantCulture)}.{exerciseSlide.Id.ToString("N").Substring(32 - 25)}.{slideTitleForMetric}";
		}

		private async Task CreateStyleErrorsReviewsForSubmission(UserExerciseSubmission submission, List<SolutionStyleError> styleErrors, string exerciseMetricId)
		{
			var ulearnBotUserId = await usersRepo.GetUlearnBotUserId();
			foreach (var error in styleErrors)
			{
				if (!await styleErrorsRepo.IsStyleErrorEnabled(error.ErrorType))
					continue;

				await slideCheckingsRepo.AddExerciseCodeReview(
					submission,
					ulearnBotUserId,
					error.Span.StartLinePosition.Line,
					error.Span.StartLinePosition.Character,
					error.Span.EndLinePosition.Line,
					error.Span.EndLinePosition.Character,
					error.Message
				);

				var errorName = Enum.GetName(typeof(StyleErrorType), error.ErrorType);
				metricSender.SendCount("exercise.style_error");
				metricSender.SendCount($"exercise.style_error.{errorName}");
				metricSender.SendCount($"exercise.{exerciseMetricId}.style_error");
				metricSender.SendCount($"exercise.{exerciseMetricId}.style_error.{errorName}");
			}
		}
	}
}