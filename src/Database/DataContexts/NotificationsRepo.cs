using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Database.Models;
using log4net;
using uLearn;

namespace Database.DataContexts
{
	public class NotificationsRepo
	{
		private const int maxNotificationsSendingFails = 15;

		private readonly ILog log = LogManager.GetLogger(typeof(NotificationsRepo));

		private readonly ULearnDb db;

		public NotificationsRepo()
			: this(new ULearnDb())
		{
		}

		public NotificationsRepo(ULearnDb db)
		{
			this.db = db;
		}

		private static DateTime CalculateNextTryTime(DateTime createTime, int failsCount)
		{
			if (failsCount == 0)
				return createTime;
			return createTime.AddSeconds(Math.Pow(2, failsCount - 1));
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

		private void DeleteOldNotificationTransports<TransportType>(string userId) where TransportType : NotificationTransport
		{
			var transports = db.NotificationTransports
				.Where(t => t.UserId == userId && !t.IsDeleted)
				.OfType<TransportType>().ToList();
			foreach (var transport in transports)
				transport.IsDeleted = true;
		}

		public async Task AddNotificationTransport(NotificationTransport transport)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				if (transport is MailNotificationTransport)
					DeleteOldNotificationTransports<MailNotificationTransport>(transport.UserId);
				if (transport is TelegramNotificationTransport)
					DeleteOldNotificationTransports<TelegramNotificationTransport>(transport.UserId);

				transport.IsDeleted = false;
				db.NotificationTransports.Add(transport);

				await db.SaveChangesAsync().ConfigureAwait(false);
				transaction.Commit();
			}
		}

		public NotificationTransport FindNotificationTransport(int transportId)
		{
			return db.NotificationTransports.Find(transportId);
		}
		public async Task EnableNotificationTransport(int transportId, bool isEnabled = true)
		{
			var transport = db.NotificationTransports.Find(transportId);
			if (transport == null)
				return;

			transport.IsEnabled = isEnabled;
			await db.SaveChangesAsync();
		}
		
		public List<NotificationTransport> GetUsersNotificationTransports(string userId, bool includeDisabled = false)
		{
			var transports = db.NotificationTransports.Where(t => t.UserId == userId && !t.IsDeleted);
			if (!includeDisabled)
				transports = transports.Where(r => r.IsEnabled);
			return transports.ToList();
		}

		public async Task SetNotificationTransportSettings(string courseId, int transportId, NotificationType type, bool isEnabled)
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
					IsEnabled = isEnabled,
				};
			}
			else
				settings.IsEnabled = isEnabled;

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
				.ToDefaultDictionary(() => null);
		}

		public async Task AddNotification(string courseId, Notification notification, string initiatedUserId)
		{
			notification.CreateTime = DateTime.Now;
			notification.InitiatedById = initiatedUserId;
			notification.CourseId = courseId;
			db.Notifications.Add(notification);

			await db.SaveChangesAsync();
		}

		public List<NotificationDelivery> GetDeliveriesForSendingNow()
		{
			var now = DateTime.Now;
			return db.NotificationDeliveries.Where(
				d => (d.NextTryTime < now || d.NextTryTime == null) && 
				d.Status == NotificationDeliveryStatus.NotSent &&
				d.FailsCount < maxNotificationsSendingFails
			).ToList();
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

		public async Task CreateDeliveries()
		{
			var notifications = db.Notifications.Where(
				n => !n.AreDeliveriesCreated && n.CreateTime < DateTime.Now.Subtract(TimeSpan.FromMinutes(1))
				).ToList();
			foreach (var notification in notifications)
			{
				var notificationType = notification.GetNotificationType();
				log.Info($"Found new notification {notificationType} #{notification.Id}");

				if (!notification.IsActual())
				{
					log.Info($"Notification #{notification.Id}: is not actual more");
					continue;
				}

				var recipientsIds = notification.GetRecipientsIds(db);
				log.Info($"Recipients list for notifiction {notification.Id}: {recipientsIds.Count} user(s)");

				if (recipientsIds.Count == 0)
					continue;
				
				var transportsSettings = db.NotificationTransportSettings
					.Include(s => s.NotificationTransport)
					.Where(s => s.CourseId == notification.CourseId &&
								s.NotificationType == notificationType &&
								recipientsIds.Contains(s.NotificationTransport.UserId)).ToList();

				var isEnabledByDefault = notificationType.IsEnabledByDefault();

				if (isEnabledByDefault)
				{
					log.Info($"Notification #{notification.Id}. This notification type has enabled default sending, so collection data for it");

					var recipientsTransports = db.NotificationTransports.Where(
						t => recipientsIds.Contains(t.UserId) &&
						t.IsEnabled
					).ToList();
					var notFoundTransports = recipientsTransports.Except(transportsSettings.Select(c => c.NotificationTransport), new NotificationTransportIdComparer());

					foreach (var transport in notFoundTransports)
					{
						transportsSettings.Add(new NotificationTransportSettings
						{
							IsEnabled = true,
							NotificationTransport = transport,
							NotificationTransportId = transport.Id,
						});
					}
				}
				
				var now = DateTime.Now;

				foreach (var transportSettings in transportsSettings)
				{
					log.Info($"Notification #{notification.Id}: add delivery to {transportSettings.NotificationTransport}, isEnabled: {transportSettings.IsEnabled}");
					if (!transportSettings.IsEnabled)
						continue;

					/* Always ignore to send notification to user initiated this notification */
					if (transportSettings.NotificationTransport.UserId == notification.InitiatedById)
						continue;

					log.Info($"Notification #{notification.Id}: add delivery to {transportSettings.NotificationTransport}, sending at {now}");
					db.NotificationDeliveries.Add(new NotificationDelivery
					{
						Notification = notification,
						NotificationTransportId = transportSettings.NotificationTransportId,
						CreateTime = now,
						NextTryTime = now,
						FailsCount = 0,
						Status = NotificationDeliveryStatus.NotSent,
					});
				}

				notification.AreDeliveriesCreated = true;
			}

			await db.SaveChangesAsync();
		}

		public async Task MarkDeliveriesAsFailed(List<NotificationDelivery> deliveries)
		{
			foreach (var delivery in deliveries)
			{
				delivery.FailsCount++;
				delivery.NextTryTime = CalculateNextTryTime(delivery.CreateTime, delivery.FailsCount);
			}
			await db.SaveChangesAsync();
		}

		public Task MarkDeliveryAsFailed(NotificationDelivery delivery)
		{
			return MarkDeliveriesAsFailed(new List<NotificationDelivery> { delivery });
		}

		public string GetSecretHashForTelegramTransport(long chatId, string chatTitle, string key)
		{
			var hasher = MD5.Create();
			var bytes = Encoding.UTF8.GetBytes(key + chatId + "&" + chatTitle + key);
			var hash = hasher.ComputeHash(bytes);

			var sb = new StringBuilder();
			foreach (var b in hash)
				sb.Append(b.ToString("x2"));
			return sb.ToString();
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