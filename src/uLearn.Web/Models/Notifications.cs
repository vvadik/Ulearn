using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uLearn.Web.Models
{
	public abstract class NotificationTransport
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[StringLength(64)]
		[Index("IDX_NotificationTransportSettings_ByUser")]
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

		public bool IsEnabled { get; set; }

		public bool IsDeleted { get; set; }
	}

	public class MailNotificationTransport : NotificationTransport
	{
		[StringLength(200)]
		public string Email { get; set; }
	}

	public class TelegramNotificationTransport : NotificationTransport
	{
		[StringLength(200)]
		public string ChatId { get; set; }
	}

	public class NotificationTransportSettings
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Index("IDX_NotificationTransportSettings_ByNotificationTransport")]
		public int NotificationTransportId { get; set; }
		public virtual NotificationTransport NotificationTransport { get; set; }

		[StringLength(100)]
		[Index("IDX_NotificationTransportSettings_ByCourse")]
		[Index("IDX_NotificationTransportSettings_ByCourseAndNofiticationType", 1)]
		public string CourseId { get; set; }

		[Index("IDX_NotificationTransportSettings_ByNotificationType")]
		[Index("IDX_NotificationTransportSettings_ByCourseAndNofiticationType", 2)]
		public NotificationType NotificationType { get; set; }

		public NotificationSendingFrequency Frequency { get; set; }

		public static DateTime GetNextWeekday(DateTime today, DayOfWeek day)
		{
			var daysToAdd = ((int) day - (int) today.DayOfWeek + 7) % 7;
			return today.AddDays(daysToAdd);
		}

		public DateTime FindSendTime(DateTime now)
		{
			if (Frequency == NotificationSendingFrequency.OnceADay)
			{
				// Send at 21:00 each day
				var today9Pm = new DateTime(now.Year, now.Month, now.Day, 21, 0, 0);
				if (now.Hour < 21)
					return today9Pm;
				return today9Pm + TimeSpan.FromDays(1);
			}

			if (Frequency == NotificationSendingFrequency.OnceAWeek)
			{
				// Send at 9:00 each monday
				var nearestMonday = GetNextWeekday(now, DayOfWeek.Monday);
				if (now.DayOfWeek == DayOfWeek.Monday && now.Hour > 9)
					nearestMonday = GetNextWeekday(now.AddDays(1), DayOfWeek.Monday);
				return new DateTime(nearestMonday.Year, nearestMonday.Month, nearestMonday.Day, 9, 0, 0);
			}

			return now;
		}
	}

	public enum NotificationSendingFrequency : byte
	{
		Disabled = 0,
		AtOnce = 1,
		OnceADay = 2,
		OnceAWeek = 3,
	}

	public class NotificationDelivery
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Index("IDX_NotificationDelivery_ByNotificationAndTransport", 1, IsUnique = true)]
		public int NotificationId { get; set; }
		public virtual Notification Notification { get; set; }

		[Index("IDX_NotificationDelivery_ByNotificationAndTransport", 2, IsUnique = true)]
		public int NotificationTransportId { get; set; }
		public virtual NotificationTransport NotificationTransport { get; set; }

		public NotificationDeliveryStatus Status { get; set; }

		public DateTime CreateTime { get; set; }

		[Index("IDX_NotificationDelivery_BySendTime")]
		public DateTime SendTime { get; set; }
	}

	public enum NotificationDeliveryStatus : byte
	{
		NotSent = 1,
		Sent = 2,
		Read = 3,
		WontSend = 4
	}

	public enum NotificationType : short
	{
		// Everybody
		SystemMessage = 1,
		InstructorMessage = 2,
		NewComment = 3,
		RepliedToYourComment = 4,
		LikedYourComment = 5,
		PassedManualExerciseChecking = 6,
		PassedManualQuizChecking = 7,
		ReceivedCertificate = 8,
		ReceivedAdditionalScore = 9,
		
		// Instructors
		JoinedToYourGroup = 101,

		// Course admins
		AddedInstructor = 201,
		CreatedGroup = 202,
		UploadedPackage = 203,
		PublishedPackage = 204,
	}

	public abstract class Notification
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[StringLength(100)]
		public string CourseId { get; set; }

		[StringLength(60)]
		public string InitiatedById { get; set; }
		public virtual ApplicationUser InitiatedBy { get; set; }

		public DateTime CreateTime { get; set; }

		public virtual ICollection<NotificationDelivery> Deliveries { get; set; }

		public abstract NotificationTypeProperties Properties { get; }
	}

	public class NotificationTypeProperties
	{
		public NotificationType Type;

		public string HumanName;

		public CourseRole MinCourseRole = CourseRole.Student;
	}

	public class SystemMessageNotification : Notification
	{
		public override NotificationTypeProperties Properties => new NotificationTypeProperties
		{
			Type = NotificationType.SystemMessage,
			HumanName = "Системные сообщения",
		};
	}

	public class InstructorMessageNotification : Notification
	{
		public override NotificationTypeProperties Properties => new NotificationTypeProperties
		{
			Type = NotificationType.InstructorMessage,
			HumanName = "Сообщения от преподавателя"
		};
	}

	public class NewCommentNotification : Notification
	{
		public override NotificationTypeProperties Properties => new NotificationTypeProperties
		{
			Type = NotificationType.NewComment,
			HumanName = "Новые комментарии",
		};
	}

	public class LikedYourCommentNotification : Notification
	{
		public override NotificationTypeProperties Properties => new NotificationTypeProperties
		{
			Type = NotificationType.LikedYourComment,
			HumanName = "Отметки «нравится» вашим комментариям",
		};
	}

	public class PassedManualExerciseCheckingNotification : Notification
	{
		public override NotificationTypeProperties Properties => new NotificationTypeProperties
		{
			Type = NotificationType.PassedManualExerciseChecking,
			HumanName = "Прохождение код-ревью",
		};
	}

	public class PassedManualQuizCheckingNotification : Notification
	{
		public override NotificationTypeProperties Properties => new NotificationTypeProperties
		{
			Type = NotificationType.PassedManualQuizChecking,
			HumanName = "Тест проверен преподавателем",
		};
	}

	public class ReceivedCertificateNotification : Notification
	{
		public override NotificationTypeProperties Properties => new NotificationTypeProperties
		{
			Type = NotificationType.ReceivedCertificate,
			HumanName = "Получен сертификат",
		};
	}

	public class ReceivedAdditionalScoreNotification : Notification
	{
		public override NotificationTypeProperties Properties => new NotificationTypeProperties
		{
			Type = NotificationType.ReceivedAdditionalScore,
			HumanName = "Преподаватель выставил дополнительные баллы",
		};
	}

	public class JoinedToYourGroupNotification : Notification
	{
		public override NotificationTypeProperties Properties => new NotificationTypeProperties
		{
			Type = NotificationType.JoinedToYourGroup,
			HumanName = "Кто-то присоединился к вашей группе",
			MinCourseRole = CourseRole.Instructor,
		};
	}

	public class AddedInstructorNotification : Notification
	{
		public override NotificationTypeProperties Properties => new NotificationTypeProperties
		{
			Type = NotificationType.AddedInstructor,
			HumanName = "Добавлен новый преподаватель",
			MinCourseRole = CourseRole.CourseAdmin,
		};
	}

	public class CreatedGroupNotification : Notification
	{
		public override NotificationTypeProperties Properties => new NotificationTypeProperties
		{
			Type = NotificationType.CreatedGroup,
			HumanName = "Создана новая группа",
			MinCourseRole = CourseRole.CourseAdmin,
		};
	}

	public class UploadedPackageNotification : Notification
	{
		public override NotificationTypeProperties Properties => new NotificationTypeProperties
		{
			Type = NotificationType.UploadedPackage,
			HumanName = "Загружен новый пакет с курсом",
			MinCourseRole = CourseRole.CourseAdmin,
		};
	}

	public class PublishedPackageNotification : Notification
	{
		public override NotificationTypeProperties Properties => new NotificationTypeProperties
		{
			Type = NotificationType.PublishedPackage,
			HumanName = "Опубликован новый пакет",
			MinCourseRole = CourseRole.CourseAdmin,
		};
	}
}