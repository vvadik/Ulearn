using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Database;
using Database.DataContexts;
using Database.Extensions;
using Database.Models;
using log4net;
using Microsoft.AspNet.Identity;
using uLearn.Extensions;
using uLearn.Web.Extensions;
using uLearn.Web.FilterAttributes;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize]
	public class AccountController : BaseUserController
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(AccountController));

		private readonly CourseManager courseManager = WebCourseManager.Instance;

		private readonly UserRolesRepo userRolesRepo;
		private readonly GroupsRepo groupsRepo;
		private readonly CertificatesRepo certificatesRepo;
		private readonly VisitsRepo visitsRepo;
		private readonly NotificationsRepo notificationsRepo;

		private readonly string telegramSecret;

		public AccountController()
		{
			userRolesRepo = new UserRolesRepo(db);
			groupsRepo = new GroupsRepo(db, courseManager);
			certificatesRepo = new CertificatesRepo(db, courseManager);
			visitsRepo = new VisitsRepo(db);
			notificationsRepo = new NotificationsRepo(db);

			telegramSecret = WebConfigurationManager.AppSettings["ulearn.telegram.webhook.secret"] ?? "";
		}

		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			return RedirectToAction("Index", "Login", new { returnUrl });
		}

		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public ActionResult List(UserSearchQueryModel queryModel)
		{
			return View(queryModel);
		}

		[ChildActionOnly]
		public ActionResult ListPartial(UserSearchQueryModel queryModel)
		{
			var userRoles = usersRepo.FilterUsers(queryModel, userManager);
			var model = GetUserListModel(userRoles);

			return PartialView("_UserListPartial", model);
		}

		private UserListModel GetUserListModel(IEnumerable<UserRolesInfo> users)
		{
			var coursesForUsers = db.UserRoles
				.GroupBy(userRole => userRole.UserId)
				.ToDictionary(
					g => g.Key,
					g => g
						.GroupBy(userRole => userRole.Role)
						.ToDictionary(
							gg => gg.Key,
							gg => gg
								.Select(userRole => userRole.CourseId.ToLower())
								.ToList()
						)
				);

			var courses = User.GetControllableCoursesId().ToList();
			var usersList = users.ToList();

			var model = new UserListModel
			{
				IsCourseAdmin = User.HasAccess(CourseRole.CourseAdmin), 
				ShowDangerEntities = User.IsSystemAdministrator(),
				Users = usersList.Select(user => GetUserModel(user, coursesForUsers, courses)).ToList(),
				UsersGroups = groupsRepo.GetUsersGroupsNamesAsStrings(courses, usersList.Select(u => u.UserId), User)
			};

			return model;
		}

		private UserModel GetUserModel(UserRolesInfo userRoles, Dictionary<string, Dictionary<CourseRole, List<string>>> coursesForUsers, List<string> courses)
		{
			var user = new UserModel(userRoles)
			{
				CoursesAccess = new Dictionary<string, ICoursesAccessListModel>
				{
					{
						LmsRoles.SysAdmin,
						new SingleCourseAccessModel
						{
							HasAccess = userRoles.Roles.Contains(LmsRoles.SysAdmin),
							ToggleUrl = Url.Action("ToggleSystemRole", new { userId = userRoles.UserId, role = LmsRoles.SysAdmin })
						}
					}
				}
			};

			if (!coursesForUsers.TryGetValue(userRoles.UserId, out var coursesForUser))
				coursesForUser = new Dictionary<CourseRole, List<string>>();

			foreach (var role in Enum.GetValues(typeof(CourseRole)).Cast<CourseRole>().Where(roles => roles != CourseRole.Student))
			{
				user.CoursesAccess[role.ToString()] = new ManyCourseAccessModel
				{
					CoursesAccesses = courses
						.Select(s => new CourseAccessModel
						{
							CourseId = s,
							HasAccess = coursesForUser.ContainsKey(role) && coursesForUser[role].Contains(s.ToLower()),
							ToggleUrl = Url.Action("ToggleRole", new { courseId = s, userId = user.UserId, role })
						})
						.ToList()
				};
			}
			return user;
		}

		private async Task NotifyAbountUserJoinedToGroup(Group group, string userId)
		{
			var notification = new JoinedToYourGroupNotification
			{
				Group = group,
				JoinedUserId = userId
			};
			await notificationsRepo.AddNotification(group.CourseId, notification, userId);
		}

		public async Task<ActionResult> JoinGroup(Guid hash)
		{
			var group = groupsRepo.FindGroupByInviteHash(hash);
			if (group == null)
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);

			if (Request.HttpMethod == "POST")
			{
				await groupsRepo.AddUserToGroup(group.Id, User.Identity.GetUserId());
				await NotifyAbountUserJoinedToGroup(group, User.Identity.GetUserId());

				return Redirect("/");
			}

			return View((object) group.Name);
		}

		[ULearnAuthorize(Roles = LmsRoles.SysAdmin)]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ToggleSystemRole(string userId, string role)
		{
			if (userId == User.Identity.GetUserId())
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			if (userManager.IsInRole(userId, role))
				await userManager.RemoveFromRoleAsync(userId, role);
			else
				await userManager.AddToRolesAsync(userId, role);
			return Content(role);
		}

		private async Task NotifyAboutNewInstructor(string courseId, string userId, string initiatedUserId)
		{
			var notification = new AddedInstructorNotification
			{
				AddedUserId = userId,
			};
			await notificationsRepo.AddNotification(courseId, notification, initiatedUserId);
		}

		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ToggleRole(string courseId, string userId, CourseRole role)
		{
			if (userManager.FindById(userId) == null || userId == User.Identity.GetUserId())
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			var enabledRole = await userRolesRepo.ToggleRole(courseId, userId, role);

			if (enabledRole && (role == CourseRole.Instructor || role == CourseRole.CourseAdmin))
				await NotifyAboutNewInstructor(courseId, userId, User.Identity.GetUserId());

			return Content(role.ToString());
		}

		[HttpPost]
		[ULearnAuthorize(Roles = LmsRoles.SysAdmin)]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteUser(string userId)
		{
			var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (user != null)
			{
				db.Users.Remove(user);
				await db.SaveChangesAsync();
			}
			return RedirectToAction("List");
		}

		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public ActionResult Info(string userName)
		{
			var user = db.Users.FirstOrDefault(u => u.Id == userName || u.UserName == userName);
			if (user == null)
				return RedirectToAction("List");

			var userCoursesIds = visitsRepo.GetUserCourses(user.Id);
			var userCourses = courseManager.GetCourses().Where(c => userCoursesIds.Contains(c.Id)).ToList();

			var certificates = certificatesRepo.GetUserCertificates(user.Id);

			return View(new UserInfoModel {
				User = user,
				GroupsNames = groupsRepo.GetUserGroupsNamesAsString(userCoursesIds.ToList(), user.Id, User, 10),
				Certificates = certificates,
				Courses = courseManager.GetCourses().ToDictionary(c => c.Id, c => c),
				UserCourses = userCourses,
			});
		}

		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public ActionResult CourseInfo(string userName, string courseId)
		{
			var user = db.Users.FirstOrDefault(u => u.Id == userName || u.UserName == userName);
			if (user == null)
				return RedirectToAction("List");
			var course = courseManager.GetCourse(courseId);
			return View(new UserCourseModel(course, user, db));
		}
		
		[AllowAnonymous]
		public ActionResult Register(string returnUrl = null)
		{
			return View(new RegisterViewModel { ReturnUrl = returnUrl });
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, Gender = model.Gender };
				var result = await userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await AuthenticationManager.LoginAsync(HttpContext, user, isPersistent: false);

					if (!await SendConfirmationEmail(user))
						model.ReturnUrl = Url.Action("Manage", "Account", new { Message = ManageMessageId.ErrorOccured });
					else if (string.IsNullOrWhiteSpace(model.ReturnUrl))
						model.ReturnUrl = Url.Action("Index", "Home");
					else
						model.ReturnUrl = this.FixRedirectUrl(model.ReturnUrl);

					metricSender.SendCount("registration.success");

					model.RegistrationFinished = true;
				}
				else
					this.AddErrors(result);
			}

			return View(model);
		}
		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
		{
			var result = await userManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
			var message = result.Succeeded ? ManageMessageId.LoginRemoved : ManageMessageId.ErrorOccured;
			return RedirectToAction("Manage", new { Message = message });
		}
		
		public ActionResult Manage(ManageMessageId? message)
		{
			ViewBag.StatusMessage = message?.GetAttribute<DisplayAttribute>().GetName();
			ViewBag.HasLocalPassword = ControllerUtils.HasPassword(userManager, User);
			ViewBag.ReturnUrl = Url.Action("Manage");
			return View();
		}
		
		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Manage(ManageUserViewModel model)
		{
			var hasPassword = ControllerUtils.HasPassword(userManager, User);
			ViewBag.HasLocalPassword = hasPassword;
			ViewBag.ReturnUrl = Url.Action("Manage");
			if (hasPassword)
			{
				if (ModelState.IsValid)
				{
					var result = await userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
					if (result.Succeeded)
					{
						return RedirectToAction("Manage", new { Message = ManageMessageId.PasswordChanged });
					}
					this.AddErrors(result);
				}
			}
			else
			{
				// User does not have a password so remove any validation errors caused by a missing OldPassword field
				var state = ModelState["OldPassword"];
				if (state != null)
				{
					state.Errors.Clear();
				}

				if (ModelState.IsValid)
				{
					var result = await userManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
					if (result.Succeeded)
					{
						return RedirectToAction("Manage", new { Message = ManageMessageId.PasswordSet });
					}
					this.AddErrors(result);
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		public async Task<ActionResult> StudentInfo()
		{
			var userId = User.Identity.GetUserId();
			var user = await userManager.FindByIdAsync(userId);
			return View(new LtiUserViewModel
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
			});
		}

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> StudentInfo(LtiUserViewModel userInfo)
		{
			var userId = User.Identity.GetUserId();
			var user = await userManager.FindByIdAsync(userId);
			user.FirstName = userInfo.FirstName;
			user.LastName = userInfo.LastName;
			user.Email = userInfo.Email;
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
			[Display(Name = "Пароль был изменен")]
			PasswordChanged,

			[Display(Name = "Пароль установлен")]
			PasswordSet,

			[Display(Name = "Внешний логин удален")]
			LoginRemoved,

			[Display(Name = "Ваша почта уже подтверждена")]
			EmailAlreadyConfirmed,

			[Display(Name = "Мы отправили вам письмо для подтверждения адреса")]
			ConfirmationEmailSent,

			[Display(Name = "Адрес электронной почты подтверждён")]
			EmailConfirmed,

			[Display(Name = "Аккаунт телеграма добавлен в ваш профиль")]
			TelegramAdded,

			[Display(Name = "У вас не указан адрес эл. почты")]
			UserHasNoEmail,

			[Display(Name = "Произошла ошибка. Если она будет повторяться, напишите нам на support@ulearn.me.")]
			ErrorOccured,
		}

		public PartialViewResult ChangeDetailsPartial()
		{
			var user = userManager.FindByName(User.Identity.Name);
			var hasPassword = ControllerUtils.HasPassword(userManager, User);

			return PartialView(new UserViewModel
			{
				Name = user.UserName,
				User = user,
				HasPassword = hasPassword,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				Gender = user.Gender,
			});
		}

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangeDetailsPartial(UserViewModel userModel)
		{
			var user = await userManager.FindByIdAsync(User.Identity.GetUserId());
			if (user == null)
			{
				AuthenticationManager.Logout(HttpContext);
				return RedirectToAction("Index", "Login");
			}
			var nameChanged = user.UserName != userModel.Name;
			if (nameChanged && await userManager.FindByNameAsync(userModel.Name) != null)
				return RedirectToAction("Manage", new { Message = ManageMessageId.ErrorOccured });
			var emailChanged = string.Compare(user.Email, userModel.Email, StringComparison.OrdinalIgnoreCase) != 0;

			user.UserName = userModel.Name;
			user.FirstName = userModel.FirstName;
			user.LastName = userModel.LastName;
			user.Email = userModel.Email;
			user.Gender = userModel.Gender;
			user.LastEdit = DateTime.Now;
			if (!string.IsNullOrEmpty(userModel.Password))
			{
				await userManager.RemovePasswordAsync(user.Id);
				await userManager.AddPasswordAsync(user.Id, userModel.Password);
			}
			await userManager.UpdateAsync(user);

			if (emailChanged)
				await ChangeEmail(user, user.Email);
			if (nameChanged)
			{
				AuthenticationManager.Logout(HttpContext);
				return RedirectToAction("Index", "Login");
			}
			return RedirectToAction("Manage");
		}

		[HttpPost]
		[ULearnAuthorize(Roles = LmsRoles.SysAdmin)]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(string newPassword, string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
				return RedirectToAction("List");
			await userManager.RemovePasswordAsync(userId);
			await userManager.AddPasswordAsync(userId, newPassword);
			return RedirectToAction("Info", new { user.UserName });
		}

		[AllowAnonymous]
		public ActionResult UserMenuPartial()
		{
			var isAuthenticated = Request.IsAuthenticated;
			var user = userManager.FindById(User.Identity.GetUserId());
			return PartialView(new UserMenuPartialViewModel
			{
				IsAuthenticated = isAuthenticated,
				User = user,
			});
		}

		public async Task<ActionResult> AddTelegram(long chatId, string chatTitle, string hash)
		{
			metricSender.SendCount("connect_telegram.try");
			var correctHash = notificationsRepo.GetSecretHashForTelegramTransport(chatId, chatTitle, telegramSecret);
			if (hash != correctHash)
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			var userId = User.Identity.GetUserId();
			await usersRepo.ChangeTelegram(userId, chatId, chatTitle);
			metricSender.SendCount("connect_telegram.success");
			await notificationsRepo.AddNotificationTransport(new TelegramNotificationTransport
			{
				UserId = userId,
				IsEnabled = true,
			});

			return RedirectToAction("Manage", new { Message = ManageMessageId.TelegramAdded });
		}

		public async Task<ActionResult> ConfirmEmail(string email, string signature)
		{
			metricSender.SendCount("email_confirmation.go_by_link_from_email");

			var userId = User.Identity.GetUserId();
			var user = await userManager.FindByIdAsync(userId);
			if (user.Email != email || user.EmailConfirmed)
				return RedirectToAction("Manage", new { Message = ManageMessageId.EmailAlreadyConfirmed });

			var correctSignature = GetEmailConfirmationSignature(email);
			if (signature != correctSignature)
				return RedirectToAction("Manage", new { Message = ManageMessageId.ErrorOccured });

			await usersRepo.ConfirmEmail(userId);
			metricSender.SendCount("email_confirmation.confirmed");

			/* Enable notification transport if it exists or create auto-enabled mail notification transport */
			var mailNotificationTransport = notificationsRepo.FindUsersNotificationTransport<MailNotificationTransport>(userId, includeDisabled: true);
			if (mailNotificationTransport != null)
				await notificationsRepo.EnableNotificationTransport(mailNotificationTransport.Id);
			else
				await notificationsRepo.AddNotificationTransport(new MailNotificationTransport
				{
					User = user,
					IsEnabled = true,
				});

			return RedirectToAction("Manage", new { Message = ManageMessageId.EmailConfirmed });
		}

		public async Task<ActionResult> SendConfirmationEmail()
		{
			var userId = User.Identity.GetUserId();
			var user = await userManager.FindByIdAsync(userId);
			if (string.IsNullOrEmpty(user.Email))
				return RedirectToAction("Manage", new { Message = ManageMessageId.UserHasNoEmail });

			if (user.EmailConfirmed)
				return RedirectToAction("Manage", new { Message = ManageMessageId.EmailAlreadyConfirmed });

			if (!await SendConfirmationEmail(user))
				return RedirectToAction("Manage", new { Message = ManageMessageId.ErrorOccured });

			return RedirectToAction("Manage");
		}

		public async Task ChangeEmail(ApplicationUser user, string email)
		{ 
			await usersRepo.ChangeEmail(user, email);
			
			/* Disable mail notification transport if exists */
			var mailNotificationTransport = notificationsRepo.FindUsersNotificationTransport<MailNotificationTransport>(user.Id);
			if (mailNotificationTransport != null)
				await notificationsRepo.EnableNotificationTransport(mailNotificationTransport.Id, isEnabled: false);

			/* Send confirmation email to the new address */
			await SendConfirmationEmail(user);
		}

		[AllowAnonymous]
		public ActionResult CheckIsEmailConfirmed()
		{
			if (! Request.IsAuthenticated)
				return new HttpStatusCodeResult(HttpStatusCode.OK);

			var userId = User.Identity.GetUserId();
			var user = usersRepo.FindUserById(userId);
			if (user.EmailConfirmed || ! user.LastConfirmationEmailTime.HasValue)
				return new HttpStatusCodeResult(HttpStatusCode.OK);

			var sentAgo = DateTime.Now.Subtract(user.LastConfirmationEmailTime.Value);
			if (sentAgo < TimeSpan.FromDays(1))
				return new HttpStatusCodeResult(HttpStatusCode.OK);

			/* If email has been sent less than 1 day ago, show popup. Double popup is disabled via cookies and javascript */
			return PartialView("EmailIsNotConfirmedPopup", user);
		}
	}
}