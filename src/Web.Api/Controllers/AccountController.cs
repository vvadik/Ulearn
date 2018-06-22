using System.Threading.Tasks;
using Database;
using Database.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Common.Extensions;
using Ulearn.Web.Api.Models.Results.Account;

namespace Ulearn.Web.Api.Controllers
{
	[Route("/account")]
	public class AccountController : BaseController
	{
		private readonly UlearnUserManager userManager;

		public AccountController(ILogger logger, WebCourseManager courseManager, UlearnDb db, UlearnUserManager userManager)
			: base(logger, courseManager, db)
		{
			this.userManager = userManager;
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Me()
		{
			var userId = User.GetUserId();
			var user = await userManager.FindByIdAsync(userId);
			return Json(new GetMeResponse {
				User = BuildShortUserInfo(user, discloseLogin: true),
			});
		}
	}
}