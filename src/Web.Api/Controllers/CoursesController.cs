using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.CourseRoles;
using Database.Repos.Groups;
using Database.Repos.Users;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Flashcards;
using Ulearn.Core.Courses.Units;
using Ulearn.Web.Api.Controllers.Slides;
using Ulearn.Web.Api.Models.Common;
using Ulearn.Web.Api.Models.Responses.Courses;
using Ulearn.Web.Api.Models.Responses.Groups;

namespace Ulearn.Web.Api.Controllers
{
	[Route("/courses")]
	public class CoursesController : BaseController
	{
		private readonly ICoursesRepo coursesRepo;
		private readonly ICourseRolesRepo courseRolesRepo;
		private readonly IUnitsRepo unitsRepo;
		private readonly IUserSolutionsRepo solutionsRepo;
		private readonly IUserQuizzesRepo userQuizzesRepo;
		private readonly IVisitsRepo visitsRepo;
		private readonly IGroupsRepo groupsRepo;
		private readonly IGroupMembersRepo groupMembersRepo;
		private readonly IGroupAccessesRepo groupAccessesRepo;
		private readonly SlideRenderer slideRenderer;

		public CoursesController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, ICoursesRepo coursesRepo,
			IUsersRepo usersRepo, ICourseRolesRepo courseRolesRepo, IUnitsRepo unitsRepo, IUserSolutionsRepo solutionsRepo,
			IUserQuizzesRepo userQuizzesRepo, IVisitsRepo visitsRepo, IGroupsRepo groupsRepo, IGroupMembersRepo groupMembersRepo,
			IGroupAccessesRepo groupAccessesRepo, SlideRenderer slideRenderer)
			: base(logger, courseManager, db, usersRepo)
		{
			this.coursesRepo = coursesRepo;
			this.courseRolesRepo = courseRolesRepo;
			this.unitsRepo = unitsRepo;
			this.solutionsRepo = solutionsRepo;
			this.userQuizzesRepo = userQuizzesRepo;
			this.visitsRepo = visitsRepo;
			this.groupsRepo = groupsRepo;
			this.groupMembersRepo = groupMembersRepo;
			this.groupAccessesRepo = groupAccessesRepo;
			this.slideRenderer = slideRenderer;
		}

		/// <summary>
		/// Список курсов
		/// </summary>
		/// <param name="role">Роль указывается, если нужно полуичить только те курсы, в которых пользователь имеет роль эту или выше</param>
		[HttpGet]
		public async Task<ActionResult<CoursesListResponse>> CoursesList([FromQuery] CourseRoleType? role = null)
		{
			if (role.HasValue && !IsAuthenticated)
				return Unauthorized();

			if (role == CourseRoleType.Student)
				return NotFound(new ErrorResponse("Role can not be student. Specify tester, instructor or courseAdmin"));

			var courses = await courseManager.GetCoursesAsync().ConfigureAwait(false);

			var isSystemAdministrator = await IsSystemAdministratorAsync().ConfigureAwait(false);

			// Фильтрация по роли. У администратора высшая роль.
			if (role.HasValue && !isSystemAdministrator)
			{
				var courseIdsAsRole = await courseRolesRepo.GetCoursesWhereUserIsInRoleAsync(UserId, role.Value).ConfigureAwait(false);
				courses = courses.Where(c => courseIdsAsRole.Contains(c.Id, StringComparer.InvariantCultureIgnoreCase));
			}

			// Неопубликованные курсы не покажем тем, кто не имеет роли в них.
			if (!isSystemAdministrator)
			{
				var visibleCourses = unitsRepo.GetVisibleCourses();
				var coursesInWhichUserHasAnyRole = await courseRolesRepo.GetCoursesWhereUserIsInRoleAsync(UserId, CourseRoleType.Tester).ConfigureAwait(false);
				courses = courses.Where(c => visibleCourses.Contains(c.Id) || coursesInWhichUserHasAnyRole.Contains(c.Id, StringComparer.OrdinalIgnoreCase));
			}

			// Администратор видит все курсы. Покажем сверху те, в которых он преподаватель.
			if (isSystemAdministrator)
			{
				var instructorCourseIds = await courseRolesRepo.GetCoursesWhereUserIsInStrictRoleAsync(UserId, CourseRoleType.Instructor).ConfigureAwait(false);
				courses = courses.OrderBy(c => !instructorCourseIds.Contains(c.Id, StringComparer.InvariantCultureIgnoreCase)).ThenBy(c => c.Title);
			}
			else
				courses = courses.OrderBy(c => c.Title);

			return new CoursesListResponse
			{
				Courses = courses
					.Select(c => new ShortCourseInfo
					{
						Id = c.Id,
						Title = c.Title,
						ApiUrl = Url.Action("CourseInfo", "Courses", new { courseId = c.Id })
					}
				).ToList()
			};
		}

		/// <summary>
		/// Информация о курсе
		/// </summary>
		/// <param name="groupId">If null, returns data for the current user, otherwise for a group</param>
		[HttpGet("{courseId}")]
		public async Task<ActionResult<CourseInfo>> CourseInfo([FromRoute]Course course, [FromQuery][CanBeNull]int? groupId = null)
		{
			if (course == null)
				return NotFound(new ErrorResponse("Course not found"));

			List<UnitInfo> units;
			var visibleUnitsIds = await unitsRepo.GetVisibleUnitIdsAsync(course, UserId);
			var visibleUnits = course.GetUnits(visibleUnitsIds);
			if (groupId == null)
			{
				var isInstructor = await courseRolesRepo.HasUserAccessToCourseAsync(UserId, course.Id, CourseRoleType.Instructor).ConfigureAwait(false);
				if (!isInstructor && visibleUnits.Count == 0)
					return NotFound(new ErrorResponse("Course not found"));

				var showInstructorsSlides = isInstructor;
				var getSlideMaxScoreFunc = await BuildGetSlideMaxScoreFunc(solutionsRepo, userQuizzesRepo, visitsRepo, groupsRepo, course, UserId);
				units = visibleUnits.Select(unit => BuildUnitInfo(course.Id, unit, showInstructorsSlides, getSlideMaxScoreFunc)).ToList();
			}
			else
			{
				var group = await groupsRepo.FindGroupByIdAsync(groupId.Value, true).ConfigureAwait(false);
				if (group == null)
					return NotFound(new ErrorResponse("Group not found"));

				async Task<bool> IsUserMemberOfGroup() => await groupMembersRepo.IsUserMemberOfGroup(groupId.Value, UserId).ConfigureAwait(false);
				async Task<bool> IsGroupVisibleForUserAsync() => await groupAccessesRepo.IsGroupVisibleForUserAsync(groupId.Value, UserId).ConfigureAwait(false);

				var isGroupAvailableForUser = await IsUserMemberOfGroup() || await IsGroupVisibleForUserAsync();
				if (!isGroupAvailableForUser)
					return NotFound(new ErrorResponse("Group not found"));

				if (visibleUnits.Count == 0)
					return NotFound(new ErrorResponse("Course not found"));
				
				var getSlideMaxScoreFunc = BuildGetSlideMaxScoreFunc(course, group);
				units = visibleUnits.Select(unit => BuildUnitInfo(course.Id, unit, false, getSlideMaxScoreFunc)).ToList();
			}

			var containsFlashcards = visibleUnits.Any(x => x.Slides.OfType<FlashcardSlide>().Any());
			var scoringSettings = GetScoringSettings(course);

			return new CourseInfo
			{
				Id = course.Id,
				Title = course.Title,
				Description = course.Settings.Description,
				Scoring = scoringSettings,
				NextUnitPublishTime = unitsRepo.GetNextUnitPublishTime(course.Id),
				Units = units,
				ContainsFlashcards = containsFlashcards
			};
		}

		private ScoringSettingsModel GetScoringSettings(Course course)
		{
			var groups = course.Settings.Scoring.Groups.Values
				.Concat(new []{course.Settings.Scoring.VisitsGroup})
				.Where(sg => sg != null)
				.Select(sg => new ScoringGroupModel
				{
					Id = sg.Id,
					Name = sg.Name,
					Abbr = sg.Abbreviation.NullIfEmptyOrWhitespace(),
					Description = sg.Description.NullIfEmptyOrWhitespace(),
					Weight = sg.Weight
				})
				.ToList();
			return new ScoringSettingsModel { Groups = groups };
		}

		private UnitInfo BuildUnitInfo(string courseId, Unit unit, bool showInstructorsSlides, Func<Slide, int> getSlideMaxScoreFunc)
		{
			var slides = unit.Slides.Select(slide => slideRenderer.BuildShortSlideInfo(courseId, slide, getSlideMaxScoreFunc, Url));
			if (showInstructorsSlides && unit.InstructorNote != null)
				slides = slides.Concat(new List<ShortSlideInfo> { slideRenderer.BuildShortSlideInfo(courseId, unit.InstructorNote.Slide, getSlideMaxScoreFunc, Url) });
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
	}
}