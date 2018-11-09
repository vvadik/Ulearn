using System;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.CourseRoles;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using uLearn;
using Ulearn.Web.Api.Models.Common;
using Ulearn.Web.Api.Models.Responses;
using Ulearn.Web.Api.Models.Responses.Courses;

namespace Ulearn.Web.Api.Controllers
{
	[Route("/courses")]
	public class CoursesController : BaseController
	{
		private readonly ICoursesRepo coursesRepo;
		private readonly ICourseRolesRepo courseRolesRepo;

		public CoursesController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, ICoursesRepo coursesRepo, IUsersRepo usersRepo, ICourseRolesRepo courseRolesRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.coursesRepo = coursesRepo;
			this.courseRolesRepo = courseRolesRepo;
		}

		/// <summary>
		/// Список курсов
		/// </summary>
		[HttpGet("")]
		public async Task<ActionResult<CoursesListResponse>> CoursesList()
		{
			var courses = await courseManager.GetCoursesAsync(coursesRepo).ConfigureAwait(false);
			return new CoursesListResponse
			{
				Courses = courses.Select(
					c => new ShortCourseInfo
					{
						Id = c.Id,
						Title = c.Title,
						ApiUrl = Url.Action(nameof(CourseInfo), "Courses", new { courseId = c.Id })
					}
				).ToList()
			};
		}

		/// <summary>
		/// Список курсов, в которых текущий пользователь имеет указанные права. Для системных администраторов возвращается список всех курсов
		/// </summary>
		[HttpGet("as/{role}")]
		public async Task<ActionResult<CoursesListResponse>> CoursesListOfInstructor(CourseRoleType role)
		{
			if (role == CourseRoleType.Student)
				return NotFound(new ErrorResponse("Role can not be student. Specify tester, instructor or courseAdmin"));
			
			var courses = await courseManager.GetCoursesAsync(coursesRepo).ConfigureAwait(false);
			if (await IsSystemAdministratorAsync().ConfigureAwait(false))
				return await CoursesList().ConfigureAwait(false);

			var instructorCourseIds = await courseRolesRepo.GetCoursesWhereUserIsInRoleAsync(UserId, role).ConfigureAwait(false);
			courses = courses.Where(c => instructorCourseIds.Contains(c.Id, StringComparer.InvariantCultureIgnoreCase));
			
			return new CoursesListResponse
			{
				Courses = courses.Select(
					c => new ShortCourseInfo
					{
						Id = c.Id,
						Title = c.Title,
						ApiUrl = Url.Action(nameof(CourseInfo), "Courses", new { courseId = c.Id })
					}
				).ToList()
			};
		}
		
		/// <summary>
		/// Информация о курсе
		/// </summary>
		[HttpGet("{courseId}")]
		public ActionResult<CourseInfo> CourseInfo(Course course)
		{
			if (course == null)
				return Json(new { status = "error", message = "Course not found" });
			
			return new CourseInfo
			{
				Id = course.Id,
				Title = course.Title,
				Units = course.Units.Select(unit => BuildUnitInfo(course.Id, unit)).ToList()
			};
		}
	}
}