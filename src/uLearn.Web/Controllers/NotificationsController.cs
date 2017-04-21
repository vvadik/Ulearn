using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.Extensions;
using uLearn.Web.FilterAttributes;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
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
				.Where(t => User.HasAccessFor(courseId, t.MinCourseRole))
				.OrderByDescending(t => t.MinCourseRole)
				.ThenBy(t => (int) t.Type)
				.ToList();

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
    }

	public class NotificationSettingsViewModel
	{
		public string CourseId { get; set; }
		public List<NotificationTypeProperties> NotificationTypes { get; set; }
		public List<NotificationTransport> Transports { get; set; }
		public DefaultDictionary<Tuple<int, NotificationType>, NotificationTransportSettings> TransportsSettings { get; set; }
	}
}