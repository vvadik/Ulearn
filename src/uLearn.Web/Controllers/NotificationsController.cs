using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
		private readonly WebCourseManager courseManager = WebCourseManager.Instance;

		public NotificationsController()
		{
			var db = new ULearnDb();
			notificationsRepo = new NotificationsRepo(db);
		}

		public ActionResult Index(string courseId)
		{
			var notificationTypes = NotificationsRepo.GetAllNotificationTypes();
			var transports = notificationsRepo.GetUsersNotificationTransports(User.Identity.GetUserId(), includeDisabled: true);
			var transportsSettings = notificationsRepo.GetNotificationTransportsSettings(courseId, transports.Select(t => t.Id).ToList());
			
			notificationTypes = notificationTypes
				.Where(t => User.HasAccessFor(courseId, t.GetMinCourseRole()))
				.OrderByDescending(t => t.GetMinCourseRole())
				.ThenBy(t => (int) t)
				.ToList();

			if (!User.IsSystemAdministrator())
				notificationTypes = notificationTypes.Where(t => !t.IsForSysAdminsOnly()).ToList();

			return View(new NotificationSettingsViewModel
			{
				CourseId = courseId,
				NotificationTypes = notificationTypes,
				Transports = transports,
				TransportsSettings = transportsSettings
			});
		}

		public async Task<ActionResult> AddMailTransport(string courseId, string email)
		{
			await notificationsRepo.AddNotificationTransport(new MailNotificationTransport
			{
				Email = email,
				UserId = User.Identity.GetUserId()
			});

			return RedirectToAction("Index", "Notifications", new { courseId = courseId });
		}

		public async Task<ActionResult> AddTelegramTransport(Guid code)
		{
			await notificationsRepo.ConfirmNotificationTransport(code, User.Identity.GetUserId());



			// TODO: Replace BasicProgramming or replace redirect to the feed page
			return RedirectToAction("Index", new { courseId = "BasicProgramming" });
		}
	}

	public class NotificationSettingsViewModel
	{
		public string CourseId { get; set; }
		public List<NotificationType> NotificationTypes { get; set; }
		public List<NotificationTransport> Transports { get; set; }
		public DefaultDictionary<Tuple<int, NotificationType>, NotificationTransportSettings> TransportsSettings { get; set; }
	}
}