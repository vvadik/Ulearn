using System;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos.CourseRoles;
using Database.Repos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Core.Courses;
using Ulearn.Web.Api.Models.Responses.Users;

namespace Ulearn.Web.Api.Controllers
{
	[Route("/users")]
	public class UsersController : BaseController
	{
		private readonly ICourseRoleUsersFilter courseRoleUsersFilter;

		public UsersController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, 
			IUsersRepo usersRepo, ICourseRoleUsersFilter courseRoleUsersFilter)
			: base(logger, courseManager, db, usersRepo)
		{
			this.courseRoleUsersFilter = courseRoleUsersFilter ?? throw new ArgumentNullException(nameof(courseRoleUsersFilter));
		}

		[HttpGet("{courseId}/instructors")]
		[Authorize(Policy = "Instructors")]
		public async Task<ActionResult<InstructorsListResponse>> InstructorsList(Course course)
		{
			if (course == null)
				return NotFound();
					
			var instructorIds = await courseRoleUsersFilter.GetListOfUsersWithCourseRoleAsync(CourseRoleType.Instructor, course.Id, includeHighRoles: true).ConfigureAwait(false);
			var instructors = await usersRepo.GetUsersByIdsAsync(instructorIds).ConfigureAwait(false);
			return new InstructorsListResponse
			{
				Instructors = instructors.Select(i => BuildShortUserInfo(i, discloseLogin: true)).ToList()
			};
		}
	}
}