using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Database;
using Database.DataContexts;
using Database.Models;
using log4net;
using Microsoft.AspNet.Identity;
using uLearn.Web.Extensions;
using uLearn.Web.FilterAttributes;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize]
	public class FeedController : JsonDataContractController
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(FeedController));

		private readonly ULearnDb db;
		private readonly CourseManager courseManager;

		private readonly NotificationsRepo notificationsRepo;
		private readonly FeedRepo feedRepo;

		private readonly FeedNotificationTransport commonFeedNotificationTransport;

		public FeedController()
			: this(new ULearnDb(), WebCourseManager.Instance)
		{
		}

		public FeedController(ULearnDb db, CourseManager courseManager)
		{
			this.db = db;
			this.courseManager = courseManager;
			notificationsRepo = new NotificationsRepo(db);
			feedRepo = new FeedRepo(db);

			commonFeedNotificationTransport = feedRepo.GetCommonFeedNotificationTransport();
		}

		protected override void OnActionExecuting(ActionExecutingContext context)
		{
			base.OnActionExecuting(context);

			var userId = User.Identity.GetUserId();
			feedRepo.AddFeedNotificationTransportIfNeeded(userId).Wait();
		}

		public ActionResult UnreadCount(string lastTimestamp)
		{
			var userId = User.Identity.GetUserId();
			DateTime? lastTimestampDateTime = null;
			if (DateTime.TryParse(lastTimestamp, out var dt))
				lastTimestampDateTime = dt;

			var unreadCountAndLastTimestamp = GetUnreadNotificationsCountAndLastTimestamp(userId, lastTimestampDateTime);

			return Json(new UnreadCountModel
			{
				Status = "ok",
				Count = unreadCountAndLastTimestamp.Item1,
				LastTimestamp = unreadCountAndLastTimestamp.Item2,
			}, JsonRequestBehavior.AllowGet);
		}

		private Tuple<int, DateTime?> GetUnreadNotificationsCountAndLastTimestamp(string userId, DateTime? from=null)
		{
			var notificationTransport = feedRepo.GetUsersFeedNotificationTransport(userId);
			if (notificationTransport == null)
				return Tuple.Create(0, (DateTime?) null);

			var realFrom = from ?? feedRepo.GetFeedViewTimestamp(userId) ?? DateTime.MinValue;
			var unreadCount = feedRepo.GetNotificationsCount(userId, realFrom, commonFeedNotificationTransport, notificationTransport);
			if (unreadCount > 0)
			{
				from = feedRepo.GetLastDeliveryTimestamp(commonFeedNotificationTransport).MaxWith(
					feedRepo.GetLastDeliveryTimestamp(notificationTransport)
				);
			}

			return Tuple.Create(unreadCount, from);
		}

		public ActionResult NotificationsTopbarPartial(bool isMobile=false)
		{
			var userId = User.Identity.GetUserId();
			var unreadCountAndLastTimestamp = GetUnreadNotificationsCountAndLastTimestamp(userId);
			return PartialView(new NotificationsTopbarPartialModel
			{
				UnreadCount = unreadCountAndLastTimestamp.Item1,
				LastViewTimestamp = unreadCountAndLastTimestamp.Item2,
				IsMobile = isMobile,
			});
		}

		public async Task<ActionResult> NotificationsPartial()
		{
			var userId = User.Identity.GetUserId();
			var notificationTransport = feedRepo.GetUsersFeedNotificationTransport(userId);

			var notifications = new List<Notification>();
			if (notificationTransport != null)
				notifications = feedRepo.GetFeedNotificationDeliveries(userId, commonFeedNotificationTransport, notificationTransport).Select(d => d.Notification).ToList();

			notifications = RemoveBlockedNotifications(notifications);

			var lastViewTimestamp = feedRepo.GetFeedViewTimestamp(userId);
			await feedRepo.UpdateFeedViewTimestamp(userId, DateTime.Now);

			return PartialView(new NotificationsPartialModel
			{
				Notifications = notifications,
				LastViewTimestamp = lastViewTimestamp,
				CourseManager = courseManager,
			});
		}

		private List<Notification> RemoveBlockedNotifications(IEnumerable<Notification> notifications)
		{
			return notifications.Where(notification => !notification.GetBlockerNotifications(db).Any()).ToList();
		}

		[DataContract]
		public class UnreadCountModel
		{
			[DataMember(Name = "status")]
			public string Status;

			[DataMember(Name = "count")]
			public int Count;

			[DataMember(Name = "last_timestamp")]
			public DateTime? LastTimestamp;
		}
	}

	public class NotificationsPartialModel
	{
		public List<Notification> Notifications { get; set; }
		public DateTime? LastViewTimestamp { get; set; }
		public CourseManager CourseManager { get; set; }
	}

	public class NotificationsTopbarPartialModel
	{
		public int UnreadCount { get; set; }

		public DateTime? LastViewTimestamp { get; set; }

		public bool IsMobile { get; set; }
	}
}