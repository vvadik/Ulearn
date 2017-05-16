using System;
using System.Threading.Tasks;
using Database;
using Database.Models;
using log4net;
using uLearn;

namespace Notifications
{
	public class NotificationSender : INotificationSender
	{
		private readonly ILog log = LogManager.GetLogger(typeof(NotificationSender));

		private readonly IEmailSender emailSender;
		private readonly ITelegramSender telegramSender;
		private readonly CourseManager courseManager;

		public NotificationSender(CourseManager courseManager, IEmailSender emailSender, ITelegramSender telegramSender)
		{
			this.emailSender = emailSender;
			this.telegramSender = telegramSender;
			this.courseManager = courseManager;
		}

		public NotificationSender(CourseManager courseManager)
			: this(courseManager, new KonturSpamEmailSender(), new TelegramSender())
		{
		}

		public NotificationSender()
			: this(WebCourseManager.Instance)
		{
		}

		private async Task SendAsync(MailNotificationTransport transport, NotificationDelivery notificationDelivery)
		{
			var notification = notificationDelivery.Notification;
			var course = courseManager.GetCourse(notification.CourseId);

			await emailSender.SendEmailAsync(
				transport.Email,
				notification.GetNotificationType().GetDisplayName(),
				notification.GetTextMessageForDelivery(transport, notificationDelivery, course),
				notification.GetHtmlMessageForDelivery(transport, notificationDelivery, course)
			);
		}

		private async Task SendAsync(TelegramNotificationTransport transport, NotificationDelivery notificationDelivery)
		{
			var notification = notificationDelivery.Notification;
			var course = courseManager.GetCourse(notification.CourseId);

			await telegramSender.SendMessageAsync(
				transport.ChatId,
				notification.GetHtmlMessageForDelivery(transport, notificationDelivery, course)
				);
		}

		public async Task SendAsync(NotificationDelivery notificationDelivery)
		{
			var transport = notificationDelivery.NotificationTransport;

			if (transport is MailNotificationTransport)
				await SendAsync(transport as MailNotificationTransport, notificationDelivery);
			else if (transport is TelegramNotificationTransport)
				await SendAsync(transport as TelegramNotificationTransport, notificationDelivery);
			else
				throw new Exception($"Unknown notification transport: {transport.GetType()}");
		}
	}
}
