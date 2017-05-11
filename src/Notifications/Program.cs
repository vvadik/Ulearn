using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.DataContexts;
using Database.Models;
using log4net;
using log4net.Config;

namespace Notifications
{
	class Program
	{
		private readonly NotificationsRepo notificationsRepo;
		public volatile bool Stopped;
		private readonly WebCourseManager courseManager;
		private readonly NotificationSender notificationSender;

		private static readonly ILog log = LogManager.GetLogger(typeof(Program));

		public Program(ULearnDb db, WebCourseManager courseManager)
		{
			notificationsRepo = new NotificationsRepo(db);

			this.courseManager = courseManager;
			notificationSender = new NotificationSender(courseManager);
		}

	    public Program()
	        : this(new ULearnDb(), WebCourseManager.Instance)
	    {
	    }

		static void Main(string[] args)
		{
		    XmlConfigurator.Configure();
			new Program().MainLoop().Wait();
		}

		public async Task MainLoop()
		{
			Stopped = false;
			while (!Stopped)
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

				await Task.Delay(TimeSpan.FromSeconds(1));
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
					await notificationsRepo.MarkDeliveryAsWontSend(delivery.Id);
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
			    }
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
