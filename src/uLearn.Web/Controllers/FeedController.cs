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

		public async Task<ActionResult> Index()
		{
			var feedNotificationsModel = await GetFeedNotificationsModel();
			return View(feedNotificationsModel);
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

		private Tuple<int, DateTime?> GetUnreadNotificationsCountAndLastTimestamp(string userId, DateTime? from = null)
		{
			var notificationTransport = feedRepo.GetUsersFeedNotificationTransport(userId);
			if (notificationTransport == null)
				return Tuple.Create(0, (DateTime?)null);

			var realFrom = from ?? feedRepo.GetFeedViewTimestamp(userId) ?? DateTime.MinValue;
			var unreadCount = feedRepo.GetNotificationsCount(userId, realFrom, notificationTransport);
			if (unreadCount > 0)
			{
				from = feedRepo.GetLastDeliveryTimestamp(notificationTransport);
			}

			return Tuple.Create(unreadCount, from);
		}

		public ActionResult NotificationsTopbarPartial(bool isMobile = false)
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
			var feedNotificationsModel = await GetFeedNotificationsModel();
			return PartialView(feedNotificationsModel);
		}

		private async Task<FeedNotificationsModel> GetFeedNotificationsModel()
		{
			var userId = User.Identity.GetUserId();
			var notificationTransport = feedRepo.GetUsersFeedNotificationTransport(userId);

			var importantNotifications = new List<Notification>();
			var commentsNotifications = new List<Notification>();
			if (notificationTransport != null)
			{
				importantNotifications = feedRepo.GetFeedNotificationDeliveries(userId, notificationTransport).Select(d => d.Notification).ToList();
				commentsNotifications = feedRepo.GetFeedNotificationDeliveries(userId, commonFeedNotificationTransport).Select(d => d.Notification).ToList();
			}

			importantNotifications = RemoveBlockedNotifications(importantNotifications).ToList();
			commentsNotifications = RemoveBlockedNotifications(commentsNotifications, importantNotifications).ToList();

			var lastViewTimestamp = feedRepo.GetFeedViewTimestamp(userId);
			await feedRepo.UpdateFeedViewTimestamp(userId, DateTime.Now);

			return new FeedNotificationsModel
			{
				ImportantNotifications = importantNotifications,
				CommentsNotifications = commentsNotifications,
				LastViewTimestamp = lastViewTimestamp,
				CourseManager = courseManager,
			};
		}

		private IEnumerable<Notification> RemoveBlockedNotifications(IReadOnlyCollection<Notification> notifications, IReadOnlyCollection<Notification> searchBlockersAlsoIn=null)
		{
			var notificationsIds = notifications.Select(n => n.Id).ToList();
			var searchBlockersAlsoInIds = searchBlockersAlsoIn?.Select(n => n.Id).ToList();
			foreach (var notification in notifications)
			{
				var blockers = notification.GetBlockerNotifications(db);
				if (blockers.Select(b => b.Id).Intersect(notificationsIds).Any())
					continue;
				if (searchBlockersAlsoInIds != null && blockers.Select(b => b.Id).Intersect(searchBlockersAlsoInIds).Any())
					continue;
				yield return notification;
			}
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

	public class FeedNotificationsModel
	{
		public List<Notification> ImportantNotifications { get; set; }
		public List<Notification> CommentsNotifications { get; set; }

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