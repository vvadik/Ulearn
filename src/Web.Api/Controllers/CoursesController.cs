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
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides.Flashcards;
using Ulearn.Web.Api.Models.Common;
using Ulearn.Web.Api.Models.Responses.Courses;

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

		public CoursesController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, ICoursesRepo coursesRepo,
			IUsersRepo usersRepo, ICourseRolesRepo courseRolesRepo, IUnitsRepo unitsRepo, IUserSolutionsRepo solutionsRepo,
			IUserQuizzesRepo userQuizzesRepo, IVisitsRepo visitsRepo, IGroupsRepo groupsRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.coursesRepo = coursesRepo;
			this.courseRolesRepo = courseRolesRepo;
			this.unitsRepo = unitsRepo;
			this.solutionsRepo = solutionsRepo;
			this.userQuizzesRepo = userQuizzesRepo;
			this.visitsRepo = visitsRepo;
			this.groupsRepo = groupsRepo;
		}

		/// <summary>
		/// Список курсов
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<CoursesListResponse>> CoursesList([FromQuery] CourseRoleType? role = null)
		{
			if (role.HasValue && !IsAuthenticated)
				return Unauthorized();

			if (role == CourseRoleType.Student)
				return NotFound(new ErrorResponse("Role can not be student. Specify tester, instructor or courseAdmin"));

			var courses = await courseManager.GetCoursesAsync(coursesRepo).ConfigureAwait(false);

			var isSystemAdministrator = await IsSystemAdministratorAsync().ConfigureAwait(false);

			if (role.HasValue && !isSystemAdministrator)
			{
				var courseIdsAsRole = await courseRolesRepo.GetCoursesWhereUserIsInRoleAsync(UserId, role.Value).ConfigureAwait(false);
				courses = courses.Where(c => courseIdsAsRole.Contains(c.Id, StringComparer.InvariantCultureIgnoreCase)).OrderBy(c => c.Title);
			}

			if (isSystemAdministrator)
			{
				var instructorCourseIds = await courseRolesRepo.GetCoursesWhereUserIsInRoleAsync(UserId, CourseRoleType.Instructor).ConfigureAwait(false);
				courses = courses.OrderBy(c => !instructorCourseIds.Contains(c.Id, StringComparer.InvariantCultureIgnoreCase)).ThenBy(c => c.Title);
			}

			return new CoursesListResponse
			{
				Courses = courses.Select(
					c => new ShortCourseInfo
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
		[HttpGet("{courseId}")]
		public async Task<ActionResult<CourseInfo>> CourseInfo(Course course)
		{
			if (course == null)
				return Json(new { status = "error", message = "Course not found" });

			var visibleUnits = unitsRepo.GetVisibleUnits(course, User);
			var containsFlashcards = course.Units.Any(x => x.Slides.OfType<FlashcardSlide>().Any());
			var isInstructor = await courseRolesRepo.HasUserAccessToCourseAsync(User.GetUserId(), course.Id, CourseRoleType.Instructor).ConfigureAwait(false);
			var showInstructorsSlides = isInstructor;
			var getSlideMaxScoreFunc = await BuildGetSlideMaxScoreFunc(solutionsRepo, userQuizzesRepo, visitsRepo, groupsRepo, course, User.GetUserId());
			var scoringSettings = GetScoringSettings(course);

			return new CourseInfo
			{
				Id = course.Id,
				Title = course.Title,
				Description = course.Settings.Description,
				Scoring = scoringSettings,
				NextUnitPublishTime = unitsRepo.GetNextUnitPublishTime(course.Id),
				Units = visibleUnits.Select(unit => BuildUnitInfo(course.Id, unit, showInstructorsSlides, getSlideMaxScoreFunc)).ToList(),
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
	}
}