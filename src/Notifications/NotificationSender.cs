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
		private readonly CourseManager courseManager;

		public NotificationSender(CourseManager courseManager, IEmailSender emailSender)
		{
			this.emailSender = emailSender;
			this.courseManager = courseManager;
		}

		public NotificationSender(CourseManager courseManager)
			: this(courseManager, new KonturSpamEmailSender())
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

		private Task SendAsync(TelegramNotificationTransport transport, NotificationDelivery notificationDelivery)
		{
			throw new NotImplementedException();
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
