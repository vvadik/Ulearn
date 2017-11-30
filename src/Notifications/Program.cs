using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.DataContexts;
using Database.Models;
using Graphite;
using log4net;
using log4net.Config;

namespace Notifications
{
	public class Program
	{
		private ULearnDb db;
		private NotificationsRepo notificationsRepo;
		public volatile bool Stopped;
		private readonly WebCourseManager courseManager;
		private readonly NotificationSender notificationSender;

		private static readonly ILog log = LogManager.GetLogger(typeof(Program));

		public Program(WebCourseManager courseManager)
		{
			this.courseManager = courseManager;
			notificationSender = new NotificationSender(courseManager);
		}

		public Program()
			: this(WebCourseManager.Instance)
		{
		}

		static void Main(string[] args)
		{
			XmlConfigurator.Configure();
			new Program().MainLoop().Wait();
		}

		public async Task MainLoop()
		{
			StaticMetricsPipeProvider.Instance.Start();

			Stopped = false;
			while (!Stopped)
			{
				db = new ULearnDb();
				notificationsRepo = new NotificationsRepo(db);

				try
				{
					await CreateDeliveries().ConfigureAwait(false);
					await SendDeliveries().ConfigureAwait(false);
				}
				catch (Exception e)
				{
					log.Error("Can\'t create deliveries or send them", e);
					log.Info("Waiting one second and repeat");
				}

				await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
			}

			StaticMetricsPipeProvider.Instance.Stop();
		}

		private async Task CreateDeliveries()
		{
			await notificationsRepo.CreateDeliveries();
		}

		private async Task SendDeliveries()
		{
			var deliveries = notificationsRepo.GetDeliveriesForSendingNow();

			var deliveryGroups = deliveries.GroupBy(d => Tuple.Create(d.NotificationTransportId, d.Notification.GetNotificationType()));
			foreach (var deliveryKvp in deliveryGroups)
			{
				var deliveriesForTransport = deliveryKvp.ToList();
				if (deliveriesForTransport.Count <= 0)
					continue;

				var transport = deliveriesForTransport[0].NotificationTransport;
				var notificationType = deliveryKvp.Key.Item2;
				await SendDeliveriesOfOneType(transport, notificationType, deliveriesForTransport);
			}
		}

		public async Task SendDeliveriesOfOneType(NotificationTransport transport, NotificationType notificationType, List<NotificationDelivery> deliveries)
		{
			if (deliveries.Count == 0)
				return;

			if (deliveries.Count == 1)
			{
				var delivery = deliveries[0];
				var course = courseManager.FindCourse(delivery.Notification.CourseId);
				if (course == null)
				{
					log.Warn($"Can't find course {delivery.Notification.CourseId}");
					await notificationsRepo.MarkDeliveryAsFailed(delivery);
					return;
				}

				try
				{
					await notificationSender.SendAsync(delivery);
					await notificationsRepo.MarkDeliveryAsSent(delivery.Id);
				}
				catch (Exception e)
				{
					log.Warn($"Can\'t send notification {delivery.NotificationId} to {delivery.NotificationTransport}: {e}. Will try later");
					await notificationsRepo.MarkDeliveryAsFailed(delivery);
				}
			}
			else
			{
				try
				{
					await notificationSender.SendAsync(deliveries);
					await notificationsRepo.MarkDeliveriesAsSent(deliveries.Select(d => d.Id).ToList());
				}
				catch (Exception e)
				{
					log.Warn($"Can\'t send multiple notifications [{string.Join(", ", deliveries.Select(d => d.NotificationId))}] to {deliveries[0].NotificationTransport}: {e}. Will try later");
					await notificationsRepo.MarkDeliveriesAsFailed(deliveries);
				}
			}
		}
	}
}