using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using uLearn;

namespace Database.DataContexts
{
	public class NotificationsRepo
	{
		private readonly ULearnDb db;

		public NotificationsRepo()
			: this(new ULearnDb())
		{
		}

		public NotificationsRepo(ULearnDb db)
		{
			this.db = db;
		}

		private static List<NotificationType> notificationTypes;

		private static void BuildNotificationTypesCache()
		{
			if (notificationTypes != null)
				return;

			notificationTypes = Enum.GetValues(typeof(NotificationType)).Cast<NotificationType>().ToList();
		}

		public static List<NotificationType> GetAllNotificationTypes()
		{
			BuildNotificationTypesCache();
			return notificationTypes;
		}

		public async Task AddNotificationTransport(NotificationTransport transport)
		{
			transport.IsEnabled = true;
			transport.IsDeleted = false;
			db.NotificationTransports.Add(transport);
			await db.SaveChangesAsync();
		}

		public async Task<TelegramNotificationTransport> RequestNewTelegramTransport(long chatId, string chatTitle)
		{
			var transport = new TelegramNotificationTransport
			{
				ChatId = chatId,
				ChatTitle = chatTitle.Substring(0, 200),
				ConfirmationCode = Guid.NewGuid(),
				IsConfirmed = false,
				UserId = null
			};
			db.NotificationTransports.Add(transport);
			await db.SaveChangesAsync().ConfigureAwait(false);

			return transport;
		}

		public async Task<NotificationTransport> ConfirmNotificationTransport(Guid confirmationCode, string userId)
		{
			var transport = db.NotificationTransports.FirstOrDefault(c => c.ConfirmationCode == confirmationCode && !c.IsConfirmed);
			if (transport == null)
				return null;

			transport.UserId = userId;
			transport.IsConfirmed = true;
			await db.SaveChangesAsync();

			return transport;
		}

		public List<NotificationTransport> GetUsersNotificationTransports(string userId, bool includeDisabled = false)
		{
			var transports = db.NotificationTransports.Where(t => t.UserId == userId && !t.IsDeleted);
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

		public async Task SendNotification(string courseId, Notification notification, string initiatedUserId, string recipientId)
		{
			await SendNotification(courseId, notification, initiatedUserId, new List<string> { recipientId });
		}

		public async Task SendNotification(string courseId, Notification notification, string initiatedUserId, IEnumerable<string> recipientsIds)
		{
			notification.CreateTime = DateTime.Now;
			notification.InitiatedById = initiatedUserId;
			notification.CourseId = courseId;
			db.Notifications.Add(notification);

			var recipientsIdsSet = new HashSet<string>(recipientsIds);

			var notificationType = notification.GetNotificationType();
			var transportsSettings = db.NotificationTransportSettings
				.Include(s => s.NotificationTransport)
				.Where(s => s.CourseId == courseId && 
							s.NotificationType == notificationType && 
							recipientsIdsSet.Contains(s.NotificationTransport.UserId)).ToList();
			var defaultFrequency = notificationType.GetDefaultFrequency();

			if (defaultFrequency != NotificationSendingFrequency.Disabled)
			{
				var recipientsTransports = db.NotificationTransports.Where(t => recipientsIdsSet.Contains(t.UserId) && t.IsEnabled).ToList();
				var notFoundTransports = recipientsTransports.Except(transportsSettings.Select(c => c.NotificationTransport), new NotificationTransportIdComparer());

				foreach (var transport in notFoundTransports)
				{
					transportsSettings.Add(new NotificationTransportSettings
					{
						Frequency = defaultFrequency,
						NotificationTransport = transport,
						NotificationTransportId = transport.Id,
					});
				}
			}

			foreach (var transportSettings in transportsSettings)
			{
				if (transportSettings.Frequency == NotificationSendingFrequency.Disabled)
					continue;

				/* Always ignore to send notification to user initiated this notification */
				if (transportSettings.NotificationTransport.UserId == initiatedUserId)
					continue;

				var sendTime = transportSettings.FindSendTime(DateTime.Now);

				db.NotificationDeliveries.Add(new NotificationDelivery
				{
					Notification = notification,
					NotificationTransportId = transportSettings.NotificationTransportId,
					CreateTime = DateTime.Now,
					SendTime = sendTime,
					Status = NotificationDeliveryStatus.NotSent,
				});
			}

			await db.SaveChangesAsync();
		}

		public List<NotificationDelivery> GetDeliveriesForSendingNow()
		{
			var now = DateTime.Now;
			return db.NotificationDeliveries.Where(d => d.SendTime < now && d.Status == NotificationDeliveryStatus.NotSent).ToList();
		}

		public async Task MarkDeliveriesAsSent(List<int> deliveriesIds)
		{
			foreach (var d in db.NotificationDeliveries.Where(d => deliveriesIds.Contains(d.Id)))
				d.Status = NotificationDeliveryStatus.Sent;
			await db.SaveChangesAsync();
		}

		private async Task SetDeliveryStatus(int deliveryId, NotificationDeliveryStatus status)
		{
			var delivery = db.NotificationDeliveries.Find(deliveryId);
			if (delivery == null)
				return;

			delivery.Status = status;
			await db.SaveChangesAsync();
		}

		public async Task MarkDeliveryAsRead(int deliveryId)
		{
			await SetDeliveryStatus(deliveryId, NotificationDeliveryStatus.Read);
		}

		public async Task MarkDeliveryAsWontSend(int deliveryId)
		{
			await SetDeliveryStatus(deliveryId, NotificationDeliveryStatus.WontSend);
		}

		public async Task MarkDeliveryAsSent(int deliveryId)
		{
			await SetDeliveryStatus(deliveryId, NotificationDeliveryStatus.Sent);
		}
	}

	class NotificationTransportIdComparer : IEqualityComparer<NotificationTransport>
	{
		public bool Equals(NotificationTransport first, NotificationTransport second)
		{
			return first.Id == second.Id;
		}
		public int GetHashCode(NotificationTransport transport)
		{
			return transport.Id.GetHashCode();
		}
	}
}