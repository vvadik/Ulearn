using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Database;
using Database.DataContexts;
using Database.Extensions;
using Database.Models;
using Microsoft.AspNet.Identity;
using uLearn.Web.Extensions;
using uLearn.Web.FilterAttributes;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize]
	public class NotificationsController : Controller
	{
		private readonly NotificationsRepo notificationsRepo;
		private readonly UsersRepo usersRepo;
		private readonly CourseManager courseManager;
		private readonly string telegramBotName;

		public NotificationsController(ULearnDb db, CourseManager courseManager)
		{
			notificationsRepo = new NotificationsRepo(db);
			usersRepo = new UsersRepo(db);

			this.courseManager = courseManager;
			telegramBotName = WebConfigurationManager.AppSettings["ulearn.telegram.botName"];
		}

		public NotificationsController() : this(new ULearnDb(), WebCourseManager.Instance)
		{
		}
		
		[AllowAnonymous]
		public ActionResult SuggestMailTransport()
		{
			if (!User.HasAccess(CourseRole.Instructor))
				return new HttpStatusCodeResult(HttpStatusCode.OK);

			var userId = User.Identity.GetUserId();
			var transports = notificationsRepo.GetUsersNotificationTransports(userId, includeDisabled: true);
			foreach (var transport in transports)
				if (transport is MailNotificationTransport)
					return new HttpStatusCodeResult(HttpStatusCode.OK);

			var user = usersRepo.FindUserById(userId);
			if (string.IsNullOrEmpty(user.Email) || ! user.EmailConfirmed)
				return new HttpStatusCodeResult(HttpStatusCode.OK);

			var mailNotificationTransport = new MailNotificationTransport
			{
				UserId = User.Identity.GetUserId(),
				IsEnabled = false,
			};
			notificationsRepo.AddNotificationTransport(mailNotificationTransport).Wait(5000);

			return PartialView(new SuggestMailTransportViewModel
			{
				Transport = mailNotificationTransport,
				TelegramBotName = telegramBotName,
			});
		}

		/* TODO (andgein): Accept only POST with CSRF token */
		public async Task<ActionResult> EnableNotificationTransport(int transportId, bool enable=true, string next="")
		{
			var transport = notificationsRepo.FindNotificationTransport(transportId);
			if (! User.Identity.IsAuthenticated || transport.UserId != User.Identity.GetUserId())
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			await notificationsRepo.EnableNotificationTransport(transportId, enable);

			if (next.IsLocalUrl(Request))
				return Redirect(next);

			return RedirectToAction("Manage", "Account");
		}

		public async Task<ActionResult> CreateMailTransport()
		{
			var mailTransport = new MailNotificationTransport
			{
				UserId = User.Identity.GetUserId(),
				IsEnabled = true,
				IsDeleted = false,
			};
			await notificationsRepo.AddNotificationTransport(mailTransport);

			return RedirectToAction("Manage", "Account");
		}

		[HttpPost]
		public async Task<ActionResult> SaveSettings(string courseId, int transportId)
		{
			var userId = User.Identity.GetUserId();
			var transport = notificationsRepo.FindNotificationTransport(transportId);
			if (transport == null || transport.UserId != userId)
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			foreach (var key in Request.Form.AllKeys)
			{
				var splittedKey = key.Split(new[] { "__" }, StringSplitOptions.None);
				if (splittedKey.Length != 2 || splittedKey[0] != "notification-settings")
					continue;
				
				var notificationTypeStr = splittedKey[1];
				int notificationTypeInt;
				if (!int.TryParse(notificationTypeStr, out notificationTypeInt))
					continue;

				NotificationType notificationType;
				try
				{
					notificationType = (NotificationType)notificationTypeInt;
				}
				catch (Exception)
				{
					continue;
				}

				var isEnabledStr = Request.Form[key];
				var isEnabled = isEnabledStr.StartsWith("true");

				await notificationsRepo.SetNotificationTransportSettings(courseId, transportId, notificationType, isEnabled);
			}

			var notificationTransportsSettings = courseManager.GetCourses().SelectMany(
				c => notificationsRepo.GetNotificationTransportsSettings(c.Id).Select(
					kvp => new {
						courseId = c.Id,
						transportId = kvp.Key.Item1,
						notificationType = kvp.Key.Item2,
						isEnabled = kvp.Value.IsEnabled
					}
				)
			);

			return Json(new { status = "ok", notificationTransportsSettings = notificationTransportsSettings });
		}
	}
	
	public class SuggestMailTransportViewModel
	{
		public MailNotificationTransport Transport { get; set; }
		public string TelegramBotName { get; set; }
	}
}