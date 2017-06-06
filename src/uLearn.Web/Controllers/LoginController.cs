using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Database.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using uLearn.Web.FilterAttributes;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class LoginController : UserControllerBase
	{
		public ActionResult Index(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}
		
		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Index(LoginViewModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.FindAsync(model.UserName, model.Password);
				if (user != null)
				{
					await AuthenticationManager.LoginAsync(HttpContext, user, model.RememberMe);
					await SendConfirmationEmailAfterLogin(user);
					return Redirect(this.FixRedirectUrl(returnUrl));
				}
				ModelState.AddModelError("", @"Неверное имя пользователя или пароль");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		private async Task SendConfirmationEmailAfterLogin(ApplicationUser user)
		{
			if (string.IsNullOrEmpty(user.Email) || user.EmailConfirmed)
				return;

			/* Reset cookie and show popup with remainder if needed */
			Response.Cookies.Add(new HttpCookie("emailIsNotConfirmed") { Value = null, Expires = DateTime.Now.AddDays(-1) });

			if (user.LastConfirmationEmailTime == null)
				await SendConfirmationEmail(user);
		}
		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
		}
		
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(HttpContext);
			if (loginInfo == null)
			{
				return RedirectToAction("Index", "Login");
			}

			// Sign in the user with this external login provider if the user already has a login
			var user = await userManager.FindAsync(loginInfo.Login);
			if (user != null)
			{
				var avatarUrl = loginInfo.ExternalIdentity.Claims.FirstOrDefault(x => x.Type == "AvatarUrl")?.Value;
				if (!string.IsNullOrEmpty(avatarUrl))
				{
					user.AvatarUrl = avatarUrl;
					await userManager.UpdateAsync(user);
				}
				await AuthenticationManager.LoginAsync(HttpContext, user, isPersistent: false);
				await SendConfirmationEmailAfterLogin(user);
				return Redirect(this.FixRedirectUrl(returnUrl));
			}

			// If the user does not have an account, then prompt the user to create an account
			ViewBag.ReturnUrl = returnUrl;
			ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
			return View("ExternalLoginConfirmation",
				new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
		}
		
		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Manage", "Account");
			}
			var info = await AuthenticationManager.GetExternalLoginInfoAsync(HttpContext);
			if (info == null)
			{
				return View("ExternalLoginFailure");
			}
			ViewBag.LoginProvider = info.Login.LoginProvider;
			if (ModelState.IsValid)
			{
				var userAvatarUrl = info.ExternalIdentity.Claims.FirstOrDefault(x => x.Type == "AvatarUrl")?.Value;
				var firstName = info.ExternalIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
				var lastName = info.ExternalIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
				var user = new ApplicationUser
				{
					UserName = model.UserName,
					FirstName = firstName,
					LastName = lastName,
					AvatarUrl = userAvatarUrl,
					Email = model.Email,
				};
				var result = await userManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await userManager.AddLoginAsync(user.Id, info.Login);
					if (result.Succeeded)
					{
						await AuthenticationManager.LoginAsync(HttpContext, user, isPersistent: false);
						return Redirect(this.FixRedirectUrl(returnUrl));
					}
				}
				this.AddErrors(result);
			}

			ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}
		
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}
		
		[HttpPost]
		[ULearnAuthorize]
		[ValidateAntiForgeryToken]
		public ActionResult LinkLogin(string provider)
		{
			// Request a redirect to the external login provider to link a login for the current user
			return new ChallengeResult(provider, Url.Action("LinkLoginCallback"), User.Identity.GetUserId());
		}
		
		[ULearnAuthorize]
		public async Task<ActionResult> LinkLoginCallback()
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(HttpContext, XsrfKey, User.Identity.GetUserId());
			if (loginInfo == null)
			{
				return RedirectToAction("Manage", "Account", new { Message = AccountController.ManageMessageId.ErrorOccured });
			}
			var result = await userManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
			if (result.Succeeded)
			{
				return RedirectToAction("Manage", "Account");
			}
			return RedirectToAction("Manage", "Account", new { Message = AccountController.ManageMessageId.ErrorOccured });
		}
		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.Logout(HttpContext);
			return RedirectToAction("Index", "Home");
		}

		private const string XsrfKey = "XsrfId";

		private class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri, string userId = null)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
	}
}