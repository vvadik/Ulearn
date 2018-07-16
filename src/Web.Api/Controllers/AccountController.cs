using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Extensions;
using Database.Models;
using Database.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Common.Extensions;
using Ulearn.Web.Api.Models.Responses.Account;

namespace Ulearn.Web.Api.Controllers
{
	[Route("/account")]
	public class AccountController : BaseController
	{
		private readonly UlearnUserManager userManager;
		private readonly UserRolesRepo userRolesRepo;

		public AccountController(ILogger logger, WebCourseManager courseManager, UlearnDb db, UlearnUserManager userManager, UserRolesRepo userRolesRepo)
			: base(logger, courseManager, db)
		{
			this.userManager = userManager;
			this.userRolesRepo = userRolesRepo;
		}

		[HttpGet]
		public async Task<IActionResult> Me()
		{
			var isAuthenticated = User.Identity.IsAuthenticated;
			if (!isAuthenticated)
				return Json(new GetMeResponse { IsAuthenticated = false });
			
			var userId = User.GetUserId();
			var user = await userManager.FindByIdAsync(userId);
			return Json(new GetMeResponse {
				IsAuthenticated = true,
				User = BuildShortUserInfo(user, discloseLogin: true),
			});
		}

		[HttpGet("roles")]
		[Authorize]
		public async Task<IActionResult> CourseRoles()
		{
			var rolesByCourse = await userRolesRepo.GetRolesAsync(User.GetUserId()).ConfigureAwait(false);
			var isSystemAdministrator = User.IsSystemAdministrator();
			return Json(new CourseRolesResponse
			{
				IsSystemAdministrator = isSystemAdministrator,
				Roles = rolesByCourse.Where(kvp => kvp.Value != CourseRole.Student).Select(kvp => new CourseRoleResponse
				{
					CourseId = kvp.Key,
					Role = kvp.Value,
				}).ToList(),
			});
		}
	}
}