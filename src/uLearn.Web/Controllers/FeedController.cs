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

		public ActionResult UnreadCount(DateTime? lastTimestamp=null)
		{
			var userId = User.Identity.GetUserId();
			var unreadCountAndLastTimestamp = GetUnreadNotificationsCountAndLastTimestamp(userId, lastTimestamp);

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

		public ActionResult NotificationsTopbarPartial()
		{
			var userId = User.Identity.GetUserId();
			var unreadCountAndLastTimestamp = GetUnreadNotificationsCountAndLastTimestamp(userId);
			return PartialView(new NotificationsTopbarPartialModel
			{
				UnreadCount = unreadCountAndLastTimestamp.Item1,
				LastViewTimestamp = unreadCountAndLastTimestamp.Item2,
			});
		}

		public ActionResult NotificationsPartial()
		{
			var userId = User.Identity.GetUserId();
			var notificationTransport = feedRepo.GetUsersFeedNotificationTransport(userId);
			var notifications = feedRepo.GetFeedNotificationDeliveries(userId, commonFeedNotificationTransport, notificationTransport).Select(d => d.Notification).ToList();
			return PartialView(new NotificationsPartialModel
			{
				Notifications = notifications,
				CourseManager = courseManager,
			});
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
		public CourseManager CourseManager { get; set; }
	}

	public class NotificationsTopbarPartialModel
	{
		public int UnreadCount { get; set; }

		public DateTime? LastViewTimestamp { get; set; }
	}
}