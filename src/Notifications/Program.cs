using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.DataContexts;
using Database.Models;
using log4net;
using Telegram.Bot;

namespace Notifications
{
	class Program
	{
		private readonly NotificationsRepo notificationsRepo;
		public volatile bool Stopped;
		private readonly TelegramBotClient telegramBot;
		private readonly WebCourseManager courseManager;

		private static readonly ILog log = LogManager.GetLogger(typeof(Program));

		public Program()
		{
			var db = new ULearnDb();
			notificationsRepo = new NotificationsRepo(db);

			var botToken = ConfigurationManager.AppSettings["ulearn.telegram.botToken"];
			telegramBot = new TelegramBotClient(botToken);
			courseManager = WebCourseManager.Instance;
		}

		static void Main(string[] args)
		{
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
					await notificationsRepo.MarkDeliveriesAsWontSend(delivery.Id);
					return;
				}

				var message = delivery.Notification.GetHtmlMessageForDelivery(transport, delivery, course);
				if (message == null)
				{
					await notificationsRepo.MarkDeliveriesAsWontSend(delivery.Id);
					return;
				}

				await transport.SendHtmlMessage(message);
			}
			else
			{
				
			}
		}
	}
}
