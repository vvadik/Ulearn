using System;
using System.Linq;
using Database;
using Database.Models;
using Database.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using uLearn;
using Ulearn.Web.Api.Models.Responses.Users;

namespace Ulearn.Web.Api.Controllers
{
	[Route("/users")]
	public class UsersController : BaseController
	{
		private readonly IUsersRepo usersRepo;
		private readonly IUserRolesRepo userRolesRepo;

		public UsersController(ILogger logger, WebCourseManager courseManager, IUsersRepo usersRepo, IUserRolesRepo userRolesRepo, UlearnDb db)
			: base(logger, courseManager, db)
		{
			this.usersRepo = usersRepo ?? throw new ArgumentNullException(nameof(usersRepo));
			this.userRolesRepo = userRolesRepo ?? throw new ArgumentNullException(nameof(userRolesRepo));
		}

		[HttpGet("{courseId}/instructors")]
		[Authorize(Policy = "Instructors")]
		public IActionResult InstructorsList(Course course)
		{
			if (course == null)
				return NotFound();
					
			var instructorIds = userRolesRepo.GetListOfUsersWithCourseRole(CourseRole.Instructor, course.Id);
			var instructors = usersRepo.GetUsersByIds(instructorIds);
			return Json(new InstructorsListResponse
			{
				Instructors = instructors.Select(i => BuildShortUserInfo(i, discloseLogin: true)).ToList()
			});
		}
	}
}