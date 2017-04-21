using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ApprovalUtilities.Reflection;
using ApprovalUtilities.SimpleLogger;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class NotificationsRepo
	{
		private readonly ULearnDb db;

		public NotificationsRepo() : this(new ULearnDb())
		{

		}

		public NotificationsRepo(ULearnDb db)
		{
			this.db = db;
		}

		private static ConcurrentDictionary<NotificationType, NotificationTypeProperties> notificationTypesProperties;

		private static void BuildNotificationTypesCache()
		{
			if (notificationTypesProperties != null)
				return;


			var allNotificationTypes = typeof(Notification).Assembly.GetTypes()
				.Where(t => t.IsSubclassOf(typeof(Notification)))
				.Select(t => (Notification) Activator.CreateInstance(t));
			notificationTypesProperties = new ConcurrentDictionary<NotificationType, NotificationTypeProperties>(allNotificationTypes.ToDictionary(
				t => t.Properties.Type,
				t => t.Properties
			));
		}

		public static NotificationTypeProperties GetNotificationTypeProperties(NotificationType type)
		{
			BuildNotificationTypesCache();
			return notificationTypesProperties[type];
		}

		public static List<NotificationTypeProperties> GetAllNotificationTypes()
		{
			BuildNotificationTypesCache();
			return notificationTypesProperties.Values.ToList();
		}

		public async Task AddNotificationTransport(NotificationTransport transport)
		{
			transport.IsEnabled = true;
			transport.IsDeleted = false;
			db.NotificationTransports.Add(transport);
			await db.SaveChangesAsync();
		}

		public List<NotificationTransport> GetUsersNotificationTransports(string userId, bool includeDisabled = false)
		{
			var transports = db.NotificationTransports.Where(t => t.UserId == userId && ! t.IsDeleted);
			if (!includeDisabled)
				transports = transports.Where(r => r.IsEnabled);
			return transports.ToList();
		}

		public async Task SetNotificationFrequence(string courseId, int transportId, NotificationType type, NotificationSendingFrequency frequency)
		{
			var settings = db.NotificationTransportSettings.FirstOrDefault(
				s => s.CourseId == courseId && s.NotificationTransportId == transportId && s.NotificationType == type
			);
			if (settings == null)
			{
				settings = new NotificationTransportSettings
				{
					CourseId = courseId,
					NotificationTransportId = transportId,
					NotificationType = type,
					Frequency = frequency,
				};
			}
			else
				settings.Frequency = frequency;

			db.NotificationTransportSettings.AddOrUpdate(settings);
			await db.SaveChangesAsync();
		}

		public DefaultDictionary<int, NotificationTransportSettings> GetNotificationTransportsSettings(string courseId, NotificationType type, List<int> transportIds)
		{
			return db.NotificationTransportSettings
				.Where(s => s.CourseId == courseId && s.NotificationType == type && transportIds.Contains(s.NotificationTransportId))
				.ToDictionary(s => s.NotificationTransportId, s => s)
				.ToDefaultDictionary();
		}

		public DefaultDictionary<Tuple<int, NotificationType>, NotificationTransportSettings> GetNotificationTransportsSettings(string courseId, List<int> transportIds)
		{
			return db.NotificationTransportSettings
				.Where(s => s.CourseId == courseId && transportIds.Contains(s.NotificationTransportId))
				.ToDictionary(s => Tuple.Create(s.NotificationTransportId, s.NotificationType), s => s)
				.ToDefaultDictionary();
		}

		public async Task SendNotification(string courseId, Notification notification, string initiatedUserId, IEnumerable<string> recipientsIds)
		{
			notification.CreateTime = DateTime.Now;
			notification.InitiatedById = initiatedUserId;
			notification.CourseId = courseId;
			db.Notifications.Add(notification);

			foreach (var recipientId in recipientsIds)
			{
				var transports = GetUsersNotificationTransports(recipientId);
				var transportsSettings = GetNotificationTransportsSettings(courseId, notification.Properties.Type, transports.Select(t => t.Id).ToList());
				foreach (var transport in transports)
				{
					var transportSettings = transportsSettings[transport.Id];
					if (transportSettings == null || transportSettings.Frequency == NotificationSendingFrequency.Disabled)
						continue;

					var sendTime = transportSettings.FindSendTime(DateTime.Now);

					notification.Deliveries.Add(new NotificationDelivery
					{
						NotificationTransportId = transport.Id,
						CreateTime = DateTime.Now,
						SendTime = sendTime,
						Status = NotificationDeliveryStatus.NotSent,
					});
				}
			}

			await db.SaveChangesAsync();
		}

		
	}
}