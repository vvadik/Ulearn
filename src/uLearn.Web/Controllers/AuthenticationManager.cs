using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class AuthenticationManager
	{
		private readonly UserManager<ApplicationUser> userManager;

		public AuthenticationManager()
		{
			userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ULearnDb()));
		}

		public static async Task LoginAsync(Controller controller, ApplicationUser user, bool isPersistent)
		{
			await new AuthenticationManager().InternalLoginAsync(controller, user, isPersistent);
		}

		private async Task InternalLoginAsync(Controller controller, ApplicationUser user, bool isPersistent)
		{
			var authenticationManager = GetAuthenticationManager(controller);
			authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
			var identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
			authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
		}

		public static void Logout(Controller controller)
		{
			GetAuthenticationManager(controller).SignOut();
		}

		public static async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(Controller controller, string xsrfKey, string userId)
		{
			return await GetAuthenticationManager(controller).GetExternalLoginInfoAsync(xsrfKey, userId);
		}

		public static async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(Controller controller)
		{
			return await GetAuthenticationManager(controller).GetExternalLoginInfoAsync();
		}

		private static IAuthenticationManager GetAuthenticationManager(Controller controller)
		{
			return controller.HttpContext.GetOwinContext().Authentication;
		}
	}
}