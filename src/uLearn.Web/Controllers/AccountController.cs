using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private readonly ULearnDb db;
		private readonly CourseManager courseManager;
		private UserManager<ApplicationUser> userManager;

		public AccountController()
			: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ULearnDb())))
		{
			db = new ULearnDb();
			courseManager = WebCourseManager.Instance;
			userManager.UserValidator =
				new UserValidator<ApplicationUser>(userManager)
				{
					AllowOnlyAlphanumericUserNames = false
				};
		}

		public AccountController(UserManager<ApplicationUser> userManager)
		{
			this.userManager = userManager;
		}

		[Authorize(Roles = LmsRoles.Admin + "," + LmsRoles.Instructor)]
		public ActionResult List(string namePrefix = null, string role = null)
		{
			IQueryable<ApplicationUser> applicationUsers = new ULearnDb().Users;
			if (!string.IsNullOrEmpty(namePrefix))
				applicationUsers = applicationUsers.Where(u => u.UserName.StartsWith(namePrefix));
			if (!string.IsNullOrEmpty(role))
				applicationUsers = applicationUsers.Where(u => u.Roles.Any(r => r.Role.Name == role));
			return View(applicationUsers.OrderBy(u => u.UserName).Take(50).ToList());
		}

		[Authorize(Roles = LmsRoles.Admin)]
		public async Task<ActionResult> ToggleRole(string userId, string role)
		{
			if (userManager.IsInRole(userId, role))
				await userManager.RemoveFromRoleAsync(userId, role);
			else
				await userManager.AddToRolesAsync(userId, role);
			return RedirectToAction("List");
		}

		[HttpPost]
		[Authorize(Roles = LmsRoles.Admin)]
		public async Task<ActionResult> DeleteUser(string userId)
		{
			ApplicationUser user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (user != null)
			{
				db.Users.Remove(user);
				await db.SaveChangesAsync();
			}
			return RedirectToAction("List");
		}

		[Authorize(Roles = LmsRoles.Admin + "," + LmsRoles.Instructor)]
		public ActionResult Info(string userName)
		{
			var user = db.Users.FirstOrDefault(u => u.Id == userName || u.UserName == userName);
			if (user == null)
				return RedirectToAction("List");
			var courses = new HashSet<string>(db.Visiters.Where(v => v.UserId == user.Id).Select(v => v.CourseId).Distinct());
			return View(new UserInfoModel(user, courseManager.GetCourses().Where(c => courses.Contains(c.Id)).ToArray()));
		}

		[Authorize(Roles = LmsRoles.Admin + "," + LmsRoles.Instructor)]
		public ActionResult CourseInfo(string userName, string courseId)
		{
			var user = db.Users.FirstOrDefault(u => u.Id == userName || u.UserName == userName);
			if (user == null)
				return RedirectToAction("List");
			var course = courseManager.GetCourse(courseId);
			return View(new UserCourseModel(course, user, db));
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			if (filterContext.Exception is HttpAntiForgeryException)
			{
				filterContext.ExceptionHandled = true;
				filterContext.Result = RedirectToAction("Index", "Login");
			}
			base.OnException(filterContext);
		}

		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register(string returnUrl = null)
		{
			return View(new RegisterViewModel { ReturnUrl = returnUrl });
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser { UserName = model.UserName };
				var result = await userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await AuthenticationManager.LoginAsync(this, user, isPersistent: false);
					if (string.IsNullOrWhiteSpace(model.ReturnUrl))
						return RedirectToAction("Index", "Home");
					return Redirect(this.FixRedirectUrl(model.ReturnUrl));
				}
				this.AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// POST: /Account/Disassociate
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
		{
			ManageMessageId? message;
			IdentityResult result =
				await userManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
			if (result.Succeeded)
			{
				message = ManageMessageId.RemoveLoginSuccess;
			}
			else
			{
				message = ManageMessageId.Error;
			}
			return RedirectToAction("Manage", new { Message = message });
		}

		//
		// GET: /Account/Manage
		public ActionResult Manage(ManageMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ManageMessageId.ChangePasswordSuccess
					? "Пароль был изменен."
					: message == ManageMessageId.SetPasswordSuccess
						? "Пароль установлен."
						: message == ManageMessageId.RemoveLoginSuccess
							? "Внешний логин удален."
							: message == ManageMessageId.Error
								? "Ошибка."
								: "";
			ViewBag.HasLocalPassword = ControllerUtils.HasPassword(userManager, User);
			ViewBag.ReturnUrl = Url.Action("Manage");
			return View();
		}

		//
		// POST: /Account/Manage
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Manage(ManageUserViewModel model)
		{
			bool hasPassword = ControllerUtils.HasPassword(userManager, User);
			ViewBag.HasLocalPassword = hasPassword;
			ViewBag.ReturnUrl = Url.Action("Manage");
			if (hasPassword)
			{
				if (ModelState.IsValid)
				{
					var result = await userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
					if (result.Succeeded)
					{
						return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
					}
					this.AddErrors(result);
				}
			}
			else
			{
				// User does not have a password so remove any validation errors caused by a missing OldPassword field
				ModelState state = ModelState["OldPassword"];
				if (state != null)
				{
					state.Errors.Clear();
				}

				if (ModelState.IsValid)
				{
					IdentityResult result = await userManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
					if (result.Succeeded)
					{
						return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
					}
					else
					{
						this.AddErrors(result);
					}
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[Authorize]
		public async Task<ActionResult> StudentInfo()
		{
			var userId = User.Identity.GetUserId();
			var user = await userManager.FindByIdAsync(userId);
			return View(new LtiUserViewModel
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				GroupName = user.GroupName
			});
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> StudentInfo(LtiUserViewModel userInfo)
		{
			var userId = User.Identity.GetUserId();
			var user = await userManager.FindByIdAsync(userId);
			user.FirstName = userInfo.FirstName;
			user.LastName = userInfo.LastName;
			user.Email = userInfo.Email;
			user.GroupName = userInfo.GroupName;
			user.LastEdit = DateTime.Now;
			await userManager.UpdateAsync(user);
			return RedirectToAction("StudentInfo");
		}


		[ChildActionOnly]
		public ActionResult RemoveAccountList()
		{
			var linkedAccounts = userManager.GetLogins(User.Identity.GetUserId());
			ViewBag.ShowRemoveButton = ControllerUtils.HasPassword(userManager, User) || linkedAccounts.Count > 1;
			return PartialView("_RemoveAccountPartial", linkedAccounts);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && userManager != null)
			{
				userManager.Dispose();
				userManager = null;
			}
			base.Dispose(disposing);
		}

		public enum ManageMessageId
		{
			ChangePasswordSuccess,
			SetPasswordSuccess,
			RemoveLoginSuccess,
			Error
		}

		[Authorize]
		public async Task<PartialViewResult> ChangeDetailsPartial()
		{
			var user = await userManager.FindByNameAsync(User.Identity.Name);
			var hasPassword = ControllerUtils.HasPassword(userManager, User);
			return PartialView(new UserViewModel
			{
				Name = user.UserName, 
				GroupName = user.GroupName, 
				UserId = user.Id, 
				HasPassword = hasPassword,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email
			});
		}

		[HttpPost]
		public async Task<ActionResult> ChangeDetailsPartial(UserViewModel userModel)
		{
			var user = await userManager.FindByIdAsync(userModel.UserId);
			if (user == null)
			{
				AuthenticationManager.Logout(this);
				return RedirectToAction("Index", "Login");
			}
			var nameChanged = user.UserName != userModel.Name;
			if (nameChanged && await userManager.FindByNameAsync(userModel.Name) != null)
				return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
			user.UserName = userModel.Name;
			user.GroupName = userModel.GroupName;
			user.FirstName = userModel.FirstName;
			user.LastName = userModel.LastName;
			user.Email = userModel.Email;
			user.LastEdit = DateTime.Now;
			if (!string.IsNullOrEmpty(userModel.Password))
			{
				await userManager.RemovePasswordAsync(user.Id);
				await userManager.AddPasswordAsync(user.Id, userModel.Password);
			}
			await userManager.UpdateAsync(user);

			if (nameChanged)
			{
				AuthenticationManager.Logout(this);
				return RedirectToAction("Index", "Login");
			}
			return RedirectToAction("Manage");
		}

		[HttpPost, Authorize(Roles = LmsRoles.Admin)]
		public async Task<ActionResult> ResetPassword(string newPassword, string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
				return RedirectToAction("List");
			await userManager.RemovePasswordAsync(userId);
			await userManager.AddPasswordAsync(userId, newPassword);
			return RedirectToAction("Info", new { user.UserName });
		}
	}
}