using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;
using uLearn.Web.Telegram;

namespace uLearn.Web.Controllers
{
	public class BaseExerciseController : Controller
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(BaseExerciseController));
		private readonly ErrorsBot errorsBot = new ErrorsBot();
		
		protected readonly CourseManager courseManager;

		protected readonly ULearnDb db = new ULearnDb();
		protected readonly UserSolutionsRepo userSolutionsRepo;
		protected readonly SlideCheckingsRepo slideCheckingsRepo;
		protected readonly GroupsRepo groupsRepo;
		protected readonly VisitsRepo visitsRepo;

		private static readonly TimeSpan executionTimeout = TimeSpan.FromSeconds(45);

		public BaseExerciseController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
			userSolutionsRepo = new UserSolutionsRepo(db);
			slideCheckingsRepo = new SlideCheckingsRepo(db);
			groupsRepo = new GroupsRepo(db);
			visitsRepo = new VisitsRepo(db);
		}
		
		private string GenerateSubmissionName(Slide exerciseSlide, string userName)
		{
			return $"{userName}: {exerciseSlide.Info.Unit.Title} - {exerciseSlide.Title}";
		}

		protected async Task<RunSolutionResult> CheckSolution(string courseId, ExerciseSlide exerciseSlide, string userCode, string userId, string userName, bool waitUntilChecked, bool compileOnWebServer)
		{
			var exerciseBlock = exerciseSlide.Exercise;

			if (compileOnWebServer)
			{
				var solution = exerciseBlock.BuildSolution(userCode);
				if (solution.HasErrors)
					return new RunSolutionResult { IsCompileError = true, ErrorMessage = solution.ErrorMessage, ExecutionServiceName = "uLearn" };
				if (solution.HasStyleIssues)
					return new RunSolutionResult { IsStyleViolation = true, ErrorMessage = solution.StyleMessage, ExecutionServiceName = "uLearn" };
			}

			var submission = await userSolutionsRepo.RunUserSolution(
				courseId, exerciseSlide.Id, userId,
				userCode, null, null, "uLearn",
				GenerateSubmissionName(exerciseSlide, userName), executionTimeout,
				waitUntilChecked
			);

			if (!waitUntilChecked)
				return new RunSolutionResult { SubmissionId = submission.Id };

			var course = courseManager.GetCourse(courseId);

			if (submission == null)
			{
				log.Error($"Не смог запустить проверку решения, никто не взял его на проверку за {executionTimeout.TotalSeconds} секунд.\nКурс «{course.Title}», слайд «{exerciseSlide.Title}» ({exerciseSlide.Id})");
				errorsBot.PostToChannel($"Не смог запустить проверку решения, никто не взял его на проверку за {executionTimeout.TotalSeconds} секунд.\nКурс «{course.Title}», слайд «{exerciseSlide.Title}» ({exerciseSlide.Id})\n\nhttps://ulearn.me/Sandbox");
				return new RunSolutionResult
				{
					IsCompillerFailure = true,
					ErrorMessage = "К сожалению, из-за большой нагрузки мы не смогли оперативно проверить ваше решение. " +
					               "Мы попробуем проверить его позже, просто подождите и обновите страницу. ",
					ExecutionServiceName = "uLearn"
				};
			}

			var automaticChecking = submission.AutomaticChecking;
			var isProhibitedUserToSendForReview = slideCheckingsRepo.IsProhibitedToSendExerciseToManualChecking(courseId, exerciseSlide.Id, userId);
			var sendToReview = exerciseBlock.RequireReview &&
								automaticChecking.IsRightAnswer &&
								!isProhibitedUserToSendForReview &&
								groupsRepo.IsManualCheckingEnabledForUser(course, userId);
			if (sendToReview)
			{
				await slideCheckingsRepo.RemoveWaitingManualExerciseCheckings(courseId, exerciseSlide.Id, userId);
				await slideCheckingsRepo.AddManualExerciseChecking(courseId, exerciseSlide.Id, userId, submission);
				await visitsRepo.MarkVisitsAsWithManualChecking(exerciseSlide.Id, userId);
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
	}
}