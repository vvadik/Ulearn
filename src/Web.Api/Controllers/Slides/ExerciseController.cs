using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.Groups;
using Database.Repos.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Ulearn.Common;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.Core;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Exercises;
using Ulearn.Core.Courses.Slides.Exercises.Blocks;
using Ulearn.Core.CSharp;
using Ulearn.Core.Metrics;
using Ulearn.Core.Telegram;
using Ulearn.Web.Api.Models.Parameters.Exercise;
using Ulearn.Web.Api.Models.Responses.Exercise;
using Vostok.Logging.Abstractions;
using AutomaticExerciseCheckingStatus = Database.Models.AutomaticExerciseCheckingStatus;

namespace Ulearn.Web.Api.Controllers.Slides
{
	public class ExerciseController : BaseController
	{
		private readonly IUserSolutionsRepo userSolutionsRepo;
		private readonly ICourseRolesRepo courseRolesRepo;
		private readonly IVisitsRepo visitsRepo;
		private readonly ISlideCheckingsRepo slideCheckingsRepo;
		private readonly IGroupsRepo groupsRepo;
		private readonly IStyleErrorsRepo styleErrorsRepo;
		private readonly MetricSender metricSender;
		private readonly IServiceScopeFactory serviceScopeFactory;
		private readonly ErrorsBot errorsBot = new ErrorsBot();
		private static ILog log => LogProvider.Get().ForContext(typeof(ExerciseController));

		public ExerciseController(IWebCourseManager courseManager, UlearnDb db, MetricSender metricSender,
			IUsersRepo usersRepo, IUserSolutionsRepo userSolutionsRepo, ICourseRolesRepo courseRolesRepo, IVisitsRepo visitsRepo,
			ISlideCheckingsRepo slideCheckingsRepo, IGroupsRepo groupsRepo,
			IStyleErrorsRepo styleErrorsRepo, IServiceScopeFactory serviceScopeFactory)
			: base(courseManager, db, usersRepo)
		{
			this.metricSender = metricSender;
			this.userSolutionsRepo = userSolutionsRepo;
			this.courseRolesRepo = courseRolesRepo;
			this.visitsRepo = visitsRepo;
			this.slideCheckingsRepo = slideCheckingsRepo;
			this.groupsRepo = groupsRepo;
			this.styleErrorsRepo = styleErrorsRepo;
			this.serviceScopeFactory = serviceScopeFactory;
		}

		[HttpPost("/slides/{courseId}/{slideId}/exercise/submit")]
		[Authorize]
		public async Task<ActionResult<RunSolutionResponse>> RunSolution(
			[FromRoute] Course course, 
			[FromRoute] Guid slideId,
			[FromBody] RunSolutionParameters parameters,
			[FromQuery] Language language)
		{
			var courseId = course.Id;

			/* Check that no checking solution by this user in last time */
			var delta = TimeSpan.FromSeconds(30);
			var halfMinuteAgo = DateTime.Now.Subtract(delta);
			if (await userSolutionsRepo.IsCheckingSubmissionByUser(courseId, slideId, User.Identity.GetUserId(), halfMinuteAgo, DateTime.MaxValue))
			{
				return Json(new RunSolutionResponse(SolutionRunStatus.Ignored)
				{
					Message = $"Ваше решение по этой задаче уже проверяется. Дождитесь окончания проверки. Вы можете отправить новое решение через {delta.Seconds} секунд."
				});
			}

			var code = parameters.Solution;
			if (code.Length > TextsRepo.MaxTextSize)
			{
				return Json(new RunSolutionResponse(SolutionRunStatus.Ignored)
				{
					Message = "Слишком длинный код"
				});
			}

			var isInstructor = await courseRolesRepo.HasUserAccessToCourseAsync(UserId, courseId, CourseRoleType.Instructor);
			var exerciseSlide = (await courseManager.FindCourseAsync(courseId))?.FindSlideById(slideId, isInstructor) as ExerciseSlide;
			if (exerciseSlide == null)
				return NotFound(new ErrorResponse("Slide not found"));

			var result = await CheckSolution(
				courseId, exerciseSlide, code, language, UserId, User.Identity.Name,
				waitUntilChecked: true, saveSubmissionOnCompileErrors: false
			).ConfigureAwait(false);

			return result;
		}

		private async Task<RunSolutionResponse> CheckSolution(string courseId, 
			ExerciseSlide exerciseSlide,
			string userCode, 
			Language language,
			string userId,
			string userName,
			bool waitUntilChecked, 
			bool saveSubmissionOnCompileErrors
			)
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
					return new RunSolutionResponse(SolutionRunStatus.CompilationError) { Message = buildResult.ErrorMessage };
			}

			var hasAutomaticChecking = exerciseBlock.HasAutomaticChecking();
			var sandbox = (exerciseSlide.Exercise as UniversalExerciseBlock)?.DockerImageName;
			var submissionId = await CreateInitialSubmission(courseId, exerciseSlide, userCode, language, userId, userName,
				hasAutomaticChecking, buildResult, serviceScopeFactory);
			UserExerciseSubmission submissionNoTracking; // Получается позже, чтобы быть максимально обновленным из базы, и чтобы не занимать память надолго и не попасть в 2 поколение

			var isCourseAdmin = await courseRolesRepo.HasUserAccessToCourseAsync(userId, courseId, CourseRoleType.CourseAdmin);

			if (buildResult.HasErrors)
			{
				submissionNoTracking = await userSolutionsRepo.FindSubmissionByIdNoTracking(submissionId);
				return new RunSolutionResponse(SolutionRunStatus.Success) { Submission = SubmissionInfo.Build(submissionNoTracking, null, isCourseAdmin) };
			}

			var executionTimeout = TimeSpan.FromSeconds(exerciseBlock.TimeLimit * 2 + 5);
			try
			{
				if (hasAutomaticChecking)
				{
					var priority = exerciseBlock is SingleFileExerciseBlock ? 10 : 0;
					await userSolutionsRepo.RunAutomaticChecking(submissionId, sandbox, executionTimeout, waitUntilChecked, priority);
				}
			}
			catch (SubmissionCheckingTimeout)
			{
				log.Error($"Не смог запустить проверку решения, никто не взял его на проверку за {executionTimeout.TotalSeconds} секунд.\nКурс «{course.Title}», слайд «{exerciseSlide.Title}» ({exerciseSlide.Id})");
				await errorsBot.PostToChannelAsync($"Не смог запустить проверку решения, никто не взял его на проверку за {executionTimeout.TotalSeconds} секунд.\nКурс «{course.Title}», слайд «{exerciseSlide.Title}» ({exerciseSlide.Id})\n\nhttps://ulearn.me/Sandbox");
				submissionNoTracking = await userSolutionsRepo.FindSubmissionByIdNoTracking(submissionId);
				var message = submissionNoTracking.AutomaticChecking.Status == AutomaticExerciseCheckingStatus.Running
					? "Решение уже проверяется."
					: "Решение ждет своей очереди на проверку, мы будем пытаться проверить его еще 10 минут.";
				return new RunSolutionResponse(SolutionRunStatus.SubmissionCheckingTimeout)
				{
					Message = $"К сожалению, мы не смогли оперативно проверить ваше решение. {message}. Просто подождите и обновите страницу.",
					Submission = SubmissionInfo.Build(submissionNoTracking, null, isCourseAdmin)
				};
			}

			submissionNoTracking = await userSolutionsRepo.FindSubmissionByIdNoTracking(submissionId);

			if (!waitUntilChecked)
			{
				metricSender.SendCount($"exercise.{exerciseMetricId}.dont_wait_result");
				// По вовзращаемому значению нельзя отличить от случая, когда никто не взял на проверку
				return new RunSolutionResponse(SolutionRunStatus.Success) { Submission = SubmissionInfo.Build(submissionNoTracking, null, isCourseAdmin) };
			}

			submissionNoTracking.Reviews = await CreateStyleErrorsReviewsForSubmission(submissionId, buildResult.StyleErrors, exerciseMetricId);
			if (!hasAutomaticChecking)
				await SendToReviewAndUpdateScore(submissionNoTracking, courseManager, slideCheckingsRepo, groupsRepo, visitsRepo, metricSender);

			var score = await visitsRepo.GetScore(courseId, exerciseSlide.Id, userId);
			var waitingForManualChecking = submissionNoTracking.ManualCheckings.Any(c => !c.IsChecked) ? true : (bool?)null;
			var prohibitFurtherManualChecking = submissionNoTracking.ManualCheckings.Any(c => c.ProhibitFurtherManualCheckings);
			var result = new RunSolutionResponse(SolutionRunStatus.Success)
			{
				Score = score,
				WaitingForManualChecking = waitingForManualChecking,
				ProhibitFurtherManualChecking = prohibitFurtherManualChecking,
				Submission = SubmissionInfo.Build(submissionNoTracking, null, isCourseAdmin)
			};
			return result;
		}

		private static async Task<int> CreateInitialSubmission(string courseId, ExerciseSlide exerciseSlide, string userCode, Language language,
			string userId, string userName, bool hasAutomaticChecking, SolutionBuildResult buildResult, IServiceScopeFactory serviceScopeFactory)
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var userSolutionsRepo = (IUserSolutionsRepo)scope.ServiceProvider.GetService(typeof(IUserSolutionsRepo));
				var compilationErrorMessage = buildResult.HasErrors ? buildResult.ErrorMessage : null;
				var submissionSandbox = (exerciseSlide.Exercise as UniversalExerciseBlock)?.DockerImageName;
				var automaticCheckingStatus = hasAutomaticChecking
					? buildResult.HasErrors
						? AutomaticExerciseCheckingStatus.Done
						: AutomaticExerciseCheckingStatus.Waiting
					: (AutomaticExerciseCheckingStatus?)null;
				return await userSolutionsRepo.AddUserExerciseSubmission(
					courseId,
					exerciseSlide.Id,
					userCode,
					compilationErrorMessage,
					null,
					userId,
					"uLearn",
					GenerateSubmissionName(exerciseSlide, userName),
					language,
					submissionSandbox,
					hasAutomaticChecking,
					automaticCheckingStatus
				);
			}
		}

		private static string GenerateSubmissionName(Slide exerciseSlide, string userName)
		{
			return $"{userName}: {exerciseSlide.Info.Unit.Title} - {exerciseSlide.Title}";
		}

		public static async Task<bool> SendToReviewAndUpdateScore(UserExerciseSubmission submissionNoTracking,
			IWebCourseManager courseManager, ISlideCheckingsRepo slideCheckingsRepo, IGroupsRepo groupsRepo, IVisitsRepo visitsRepo, MetricSender metricSender)
		{
			var userId = submissionNoTracking.UserId;
			var courseId = submissionNoTracking.CourseId;
			var course = await courseManager.GetCourseAsync(courseId);
			var exerciseSlide = course.FindSlideById(submissionNoTracking.SlideId, true) as ExerciseSlide; // SlideId проверен в вызывающем методе 
			if (exerciseSlide == null)
				return false;
			var exerciseMetricId = GetExerciseMetricId(courseId, exerciseSlide);
			var automaticCheckingNoTracking = submissionNoTracking.AutomaticChecking;
			var isProhibitedUserToSendForReview = await slideCheckingsRepo.IsProhibitedToSendExerciseToManualChecking(courseId, exerciseSlide.Id, userId);
			var sendToReview = exerciseSlide.Scoring.RequireReview
								&& submissionNoTracking.AutomaticCheckingIsRightAnswer
								&& !isProhibitedUserToSendForReview
								&& await groupsRepo.IsManualCheckingEnabledForUserAsync(course, userId);
			if (sendToReview)
			{
				await slideCheckingsRepo.RemoveWaitingManualCheckings<ManualExerciseChecking>(courseId, exerciseSlide.Id, userId, false);
				await slideCheckingsRepo.AddManualExerciseChecking(courseId, exerciseSlide.Id, userId, submissionNoTracking.Id);
				await visitsRepo.MarkVisitsAsWithManualChecking(courseId, exerciseSlide.Id, userId);
				metricSender.SendCount($"exercise.{exerciseMetricId}.sent_to_review");
				metricSender.SendCount("exercise.sent_to_review");
			}

			await visitsRepo.UpdateScoreForVisit(courseId, exerciseSlide, userId);

			if (automaticCheckingNoTracking != null)
			{
				var verdictForMetric = automaticCheckingNoTracking.GetVerdict().Replace(" ", "");
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

		private async Task<List<ExerciseCodeReview>> CreateStyleErrorsReviewsForSubmission(int? submissionId, List<SolutionStyleError> styleErrors, string exerciseMetricId)
		{
			var ulearnBotUserId = await usersRepo.GetUlearnBotUserId();
			var result = new List<ExerciseCodeReview>();
			foreach (var error in styleErrors)
			{
				if (!await styleErrorsRepo.IsStyleErrorEnabled(error.ErrorType))
					continue;

				var review = await slideCheckingsRepo.AddExerciseCodeReview(
					submissionId,
					ulearnBotUserId,
					error.Span.StartLinePosition.Line,
					error.Span.StartLinePosition.Character,
					error.Span.EndLinePosition.Line,
					error.Span.EndLinePosition.Character,
					error.Message
				);
				result.Add(review);

				var errorName = Enum.GetName(typeof(StyleErrorType), error.ErrorType);
				metricSender.SendCount("exercise.style_error");
				metricSender.SendCount($"exercise.style_error.{errorName}");
				metricSender.SendCount($"exercise.{exerciseMetricId}.style_error");
				metricSender.SendCount($"exercise.{exerciseMetricId}.style_error.{errorName}");
			}
			return result;
		}
	}
}