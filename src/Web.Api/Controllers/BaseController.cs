using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Extensions;
using Database.Models;
using Database.Models.Comments;
using Database.Repos;
using Database.Repos.CourseRoles;
using Database.Repos.Groups;
using Database.Repos.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Serilog;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.Courses.Slides.Blocks.Api;
using Ulearn.Core.Courses.Slides.Exercises;
using Ulearn.Core.Courses.Slides.Flashcards;
using Ulearn.Core.Courses.Slides.Quizzes;
using Ulearn.Core.Courses.Slides.Quizzes.Blocks;
using Ulearn.Core.Courses.Units;
using Ulearn.Web.Api.Controllers.Groups;
using Ulearn.Web.Api.Models.Common;
using Ulearn.Web.Api.Models.Responses.Groups;
using Ulearn.Web.Api.Models.Responses.Notifications;

namespace Ulearn.Web.Api.Controllers
{
	/* See https://docs.microsoft.com/en-us/aspnet/core/web-api/index?view=aspnetcore-2.1#annotate-class-with-apicontrollerattribute
	   for detailed description of ApiControllerAttribute */
	[ApiController]
	[Produces("application/json")]
	public class BaseController : Controller
	{
		protected readonly ILogger logger;
		protected readonly IWebCourseManager courseManager;
		protected readonly UlearnDb db;
		protected readonly IUsersRepo usersRepo;

		protected string UserId => User.GetUserId();
		protected bool IsAuthenticated => User.Identity.IsAuthenticated;

		public BaseController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo)
		{
			this.logger = logger ?? throw new ArgumentException(nameof(logger));
			this.courseManager = courseManager ?? throw new ArgumentException(nameof(courseManager));
			this.db = db ?? throw new ArgumentException(nameof(db));
			this.usersRepo = usersRepo ?? throw new ArgumentException(nameof(usersRepo));
		}

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			DisableEfChangesTrackingForGetRequests(context);

			await next().ConfigureAwait(false);
		}

		private void DisableEfChangesTrackingForGetRequests(ActionContext context)
		{
			/* Disable change tracking in EF Core for GET requests due to performance issues */
			/* TODO (andgein): we need a way to enable change tracking for some GET requests in future */
			var isRequestSafe = context.HttpContext.Request.Method == "GET"; // Maybe for HEAD and OPTION requests too?
			db.ChangeTracker.AutoDetectChangesEnabled = !isRequestSafe;

			if (isRequestSafe)
			{
				logger.Debug("Выключаю автоматическое отслеживание изменений в EF Core: db.ChangeTracker.AutoDetectChangesEnabled = false");
			}
		}

		protected async Task<bool> IsSystemAdministratorAsync()
		{
			var user = await usersRepo.FindUserByIdAsync(UserId).ConfigureAwait(false);
			return usersRepo.IsSystemAdministrator(user);
		}

		protected UnitInfo BuildUnitInfo(string courseId, Unit unit, bool showInstructorsSlides, Func<Slide, int> getSlideMaxScoreFunc)
		{
			var slides = unit.Slides.Select(slide => BuildShortSlideInfo(courseId, slide, getSlideMaxScoreFunc));
			if (showInstructorsSlides && unit.InstructorNote != null)
				slides = slides.Concat(new List<ShortSlideInfo> { BuildShortSlideInfo(courseId, unit.InstructorNote.Slide, getSlideMaxScoreFunc) });
			return BuildUnitInfo(unit, slides);
		}

		private static UnitInfo BuildUnitInfo(Unit unit, IEnumerable<ShortSlideInfo> slides)
		{
			return new UnitInfo
			{
				Id = unit.Id,
				Title = unit.Title,
				Slides = slides.ToList(),
				AdditionalScores = GetAdditionalScores(unit)
			};
		}

		private static List<UnitScoringGroupInfo> GetAdditionalScores(Unit unit)
		{
			return unit.Settings.Scoring.Groups.Values.Where(g => g.CanBeSetByInstructor).Select(g => new UnitScoringGroupInfo(g)).ToList();
		}

		protected ApiSlideInfo BuildSlideInfo(string courseId, Slide slide, Func<Slide, int> getSlideMaxScoreFunc, bool removeHiddenBlocks)
		{
			var result = BuildShortSlideInfo<ApiSlideInfo>(courseId, slide, getSlideMaxScoreFunc);
			var baseUrl = CourseUnitUtils.GetDirectoryRelativeWebPath(slide.Info.SlideFile);
			var apiSlideBlockBuildingContext = new ApiSlideBlockBuildingContext(courseId, slide.Id, baseUrl, removeHiddenBlocks);
			result.Blocks = slide.Blocks
				.SelectMany(b => b.ToApiSlideBlocks(apiSlideBlockBuildingContext))
				.ToArray();
			return result;
		}

		protected ShortSlideInfo BuildShortSlideInfo(string courseId, Slide slide, Func<Slide, int> getSlideMaxScoreFunc)
		{
			return BuildShortSlideInfo<ShortSlideInfo>(courseId, slide, getSlideMaxScoreFunc);
		}

		protected T BuildShortSlideInfo<T>(string courseId, Slide slide, Func<Slide, int> getSlideMaxScoreFunc)
			where T : ShortSlideInfo, new()
		{
			return new T
			{
				Id = slide.Id,
				Title = slide.Title,
				Slug = slide.Url,
				ApiUrl = Url.Action("SlideInfo", "Slides", new { courseId = courseId, slideId = slide.Id }),
				MaxScore = getSlideMaxScoreFunc(slide),
				ScoringGroup = slide.ScoringGroup,
				Type = GetSlideType(slide),
				QuestionsCount = slide.Blocks.OfType<AbstractQuestionBlock>().Count(),

				// TODO: кол-во попыток
			};
		}

		protected static SlideType GetSlideType(Slide slide)
		{
			switch (slide)
			{
				case ExerciseSlide _:
					return SlideType.Exercise;
				case QuizSlide _:
					return SlideType.Quiz;
				case FlashcardSlide _:
					return SlideType.Flashcards;
				default:
					return SlideType.Lesson;
			}
		}

		protected ShortUserInfo BuildShortUserInfo(ApplicationUser user, bool discloseLogin = false, bool discloseEmail = false)
		{
			return new ShortUserInfo
			{
				Id = user.Id,
				Login = discloseLogin ? user.UserName : null,
				Email = discloseEmail ? user.Email : null,
				FirstName = user.FirstName ?? "",
				LastName = user.LastName ?? "",
				VisibleName = user.VisibleName,
				AvatarUrl = user.AvatarUrl,
				Gender = user.Gender,
			};
		}

		protected NotificationCommentInfo BuildNotificationCommentInfo(Comment comment)
		{
			if (comment == null)
				return null;

			return new NotificationCommentInfo
			{
				Id = comment.Id,
				CourseId = comment.CourseId,
				SlideId = comment.SlideId,
				PublishTime = comment.PublishTime,
				Author = BuildShortUserInfo(comment.Author),
				Text = comment.Text,
			};
		}

		protected ShortGroupInfo BuildShortGroupInfo(Group g)
		{
			return new ShortGroupInfo
			{
				Id = g.Id,
				Name = g.Name,
				CourseId = g.CourseId,
				IsArchived = g.IsArchived,
				ApiUrl = Url.Action(new UrlActionContext { Action = nameof(GroupController.Group), Controller = "Group", Values = new { groupId = g.Id } })
			};
		}

		public static async Task<Func<Slide, int>> BuildGetSlideMaxScoreFunc(IUserSolutionsRepo solutionsRepo, IUserQuizzesRepo userQuizzesRepo, IVisitsRepo visitsRepo, IGroupsRepo groupsRepo,
			Course course, string userId)
		{
			var solvedSlidesIds = GetSolvedSlides(solutionsRepo, userQuizzesRepo, course, userId);
			var slidesWithUsersManualChecking = visitsRepo.GetSlidesWithUsersManualChecking(course.Id, userId).ToImmutableHashSet();
			var enabledManualCheckingForUser = await groupsRepo.IsManualCheckingEnabledForUserAsync(course, userId).ConfigureAwait(false);
			return s => GetMaxScoreForUsersSlide(s, solvedSlidesIds.Contains(s.Id), slidesWithUsersManualChecking.Contains(s.Id), enabledManualCheckingForUser);
		}
		
		public static Func<Slide, int> BuildGetSlideMaxScoreFunc(Course course, Group group)
		{
			var enabledManualCheckingForGroup = course.Settings.IsManualCheckingEnabled || group.IsManualCheckingEnabled;
			return s => GetMaxScoreForUsersSlide(s, false, false, enabledManualCheckingForGroup);
		}

		public static HashSet<Guid> GetSolvedSlides(IUserSolutionsRepo solutionsRepo, IUserQuizzesRepo userQuizzesRepo, Course course, string userId)
		{
			var solvedSlides = solutionsRepo.GetIdOfPassedSlides(course.Id, userId);
			solvedSlides.UnionWith(userQuizzesRepo.GetPassedSlideIds(course.Id, userId));
			return solvedSlides;
		}

		public static int GetMaxScoreForUsersSlide(Slide slide, bool isSolved, bool hasManualChecking, bool enabledManualCheckingForUser)
		{
			var isExerciseOrQuiz = slide is ExerciseSlide || slide is QuizSlide;

			if (!isExerciseOrQuiz)
				return slide.MaxScore;

			if (isSolved)
				return hasManualChecking ? slide.MaxScore : GetMaxScoreWithoutManualChecking(slide);
			return enabledManualCheckingForUser ? slide.MaxScore : GetMaxScoreWithoutManualChecking(slide);
		}

		public static int GetMaxScoreWithoutManualChecking(Slide slide)
		{
			switch (slide)
			{
				case ExerciseSlide exerciseSlide:
					return exerciseSlide.Scoring.PassedTestsScore;
				case QuizSlide quizSlide:
					return quizSlide.ManualChecking ? 0 : quizSlide.MaxScore;
				default:
					return slide.MaxScore;
			}
		}
	}
}