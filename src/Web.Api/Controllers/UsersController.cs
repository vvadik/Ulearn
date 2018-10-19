using System;
using System.Linq;
using System.Threading.Tasks;
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
		public async Task<ActionResult<InstructorsListResponse>> InstructorsList(Course course)
		{
			if (course == null)
				return NotFound();
					
			var instructorIds = await userRolesRepo.GetListOfUsersWithCourseRoleAsync(CourseRole.Instructor, course.Id).ConfigureAwait(false);
			var instructors = await usersRepo.GetUsersByIdsAsync(instructorIds).ConfigureAwait(false);
			return new InstructorsListResponse
			{
				Instructors = instructors.Select(i => BuildShortUserInfo(i, discloseLogin: true)).ToList()
			};
		}
	}
}