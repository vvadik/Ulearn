using System.Security.Principal;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public static class ControllerUtils
	{
		public static bool HasPassword(UserManager<ApplicationUser> userManager, IPrincipal principal)
		{
			var user = userManager.FindById(principal.Identity.GetUserId());
			return user != null && user.PasswordHash != null;
		}

		public static string FixRedirectUrl(this Controller controller, string returnUrl)
		{
			return controller.Url.IsLocalUrl(returnUrl) ? returnUrl : controller.Url.Action("Index", "Home");
		}

		public static void AddErrors(this Controller controller, IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				controller.ModelState.AddModelError("", error);
			}
		}
	}
}