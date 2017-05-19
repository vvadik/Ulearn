using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Database;
using Database.DataContexts;
using Database.Extensions;
using Database.Models;
using Microsoft.AspNet.Identity;
using uLearn.Web.FilterAttributes;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize]
	public class NotificationsController : Controller
	{
		private readonly NotificationsRepo notificationsRepo;
		private readonly UsersRepo usersRepo;
		private readonly CourseManager courseManager;

		public NotificationsController(ULearnDb db, CourseManager courseManager)
		{
			notificationsRepo = new NotificationsRepo(db);
			usersRepo = new UsersRepo(db);

			this.courseManager = courseManager;
		}

		public NotificationsController() : this(new ULearnDb(), WebCourseManager.Instance)
		{
		}

		public ActionResult Settings(string courseId)
		{
			if (string.IsNullOrEmpty(courseId))
			{
				var coursesTitles = courseManager.GetCourses().ToDictionary(c => c.Id, c => c.Title);
				return View("SelectCourse", new NotificationsSelectCourseViewModel
				{
					CoursesTitles = coursesTitles,
				});
			}

			var course = courseManager.GetCourse(courseId);
			var notificationTypes = NotificationsRepo.GetAllNotificationTypes();
			var userId = User.Identity.GetUserId();

			var transports = notificationsRepo.GetUsersNotificationTransports(userId, includeDisabled: true);
			var transportsSettings = notificationsRepo.GetNotificationTransportsSettings(courseId, transports.Select(t => t.Id).ToList());
			
			notificationTypes = notificationTypes
				.Where(t => User.HasAccessFor(courseId, t.GetMinCourseRole()))
				.OrderByDescending(t => t.GetMinCourseRole())
				.ThenBy(t => (int) t)
				.ToList();

			if (!User.IsSystemAdministrator())
				notificationTypes = notificationTypes.Where(t => !t.IsForSysAdminsOnly()).ToList();

			var mailTransport = transports.FirstOrDefault(t => t is MailNotificationTransport) as MailNotificationTransport;
			var telegramTransport = transports.FirstOrDefault(t => t is TelegramNotificationTransport) as TelegramNotificationTransport;

			var user = usersRepo.FindUserById(userId);

			return View(new NotificationSettingsViewModel
			{
				User = user,
				Course = course,
				NotificationTypes = notificationTypes,
				TransportsSettings = transportsSettings,
				MailTransport = mailTransport,
				TelegramTransport = telegramTransport,
			});
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
			if (string.IsNullOrEmpty(user.Email))
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
			});
		}

		public async Task<ActionResult> EnableNotificationTransport(int transportId, bool enable = true, string courseId = "")
		{
			var transport = notificationsRepo.FindNotificationTransport(transportId);
			if (! User.Identity.IsAuthenticated || transport.UserId != User.Identity.GetUserId())
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			await notificationsRepo.EnableNotificationTransport(transportId, enable);

			if (string.IsNullOrEmpty(courseId))
				return RedirectToAction("Index", "Home");
			return RedirectToAction("Settings", new { courseId = courseId });
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

			return RedirectToAction("Settings");
		}

		[HttpPost]
		public async Task<ActionResult> SaveSettings(string courseId)
		{
			var userId = User.Identity.GetUserId();
			foreach (var key in Request.Form.AllKeys)
			{
				var splittedKey = key.Split(new[] { "__" }, StringSplitOptions.None);
				if (splittedKey.Length != 3 || splittedKey[0] != "notifications")
					continue;

				var transportIdStr = splittedKey[1];
				int transportId;
				if (!int.TryParse(transportIdStr, out transportId))
					continue;

				var notificationTypeStr = splittedKey[2];
				int notificationTypeInt;
				if (!int.TryParse(notificationTypeStr, out notificationTypeInt))
					continue;

				var transport = notificationsRepo.FindNotificationTransport(transportId);
				if (transport == null || transport.UserId != userId)
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

			return RedirectToAction("Settings", new { courseId = courseId });
		}
	}

	public class NotificationsSelectCourseViewModel
	{
		public Dictionary<string, string> CoursesTitles { get; set; }
	}

	public class NotificationSettingsViewModel
	{
		public ApplicationUser User { get; set; }
		public Course Course { get; set; }

		public List<NotificationType> NotificationTypes { get; set; }
		public DefaultDictionary<Tuple<int, NotificationType>, NotificationTransportSettings> TransportsSettings { get; set; }
		public MailNotificationTransport MailTransport { get; set; }
		public TelegramNotificationTransport TelegramTransport { get; set; }
	}

	public class SuggestMailTransportViewModel
	{
		public MailNotificationTransport Transport { get; set; }
	}
}