using System.Security.Principal;
using Microsoft.AspNet.Identity;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public static class ControllerUtils
	{
		public static bool HasPassword(UserManager<ApplicationUser> userManager, IPrincipal principal)
		{
			var user = userManager.FindById(principal.Identity.GetUserId());
			if (user != null)
			{
				return user.PasswordHash != null;
			}
			return false;
		}
	}
}