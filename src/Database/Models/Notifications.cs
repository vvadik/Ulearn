using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects;
using System.Reflection;
using System.Threading.Tasks;
using uLearn;
using uLearn.Extensions;

namespace Database.Models
{
	public abstract class NotificationTransport
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[StringLength(64)]
		[Index("IDX_NotificationTransport_ByUser")]
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

		public bool IsEnabled { get; set; }

		public bool IsDeleted { get; set; }

		[Index("IDX_NotificationTransport_ByConfirmationCode")]
		public Guid ConfirmationCode { get; set; }

		public bool IsConfirmed { get; set; }
	}

	public class MailNotificationTransport : NotificationTransport
	{
		[StringLength(200)]
		public string Email { get; set; }
	}

	public class TelegramNotificationTransport : NotificationTransport
	{
		[Index("IDX_TelegramNotificationTransport_ByChatId")]
		public long ChatId { get; set; }

		[StringLength(200)]
		public string ChatTitle { get; set; }
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
			var daysToAdd = ((int)day - (int)today.DayOfWeek + 7) % 7;
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
		[Display(Name = "Выключено")]
		Disabled = 0,

		[Display(Name = "Сразу")]
		AtOnce = 1,

		[Display(Name = "Раз в день")]
		OnceADay = 2,

		[Display(Name = "Раз в неделю")]
		OnceAWeek = 3,
	}

	public class NotificationDelivery
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Index("IDX_NotificationDelivery_ByNotificationAndTransport", 1)]
		public int NotificationId { get; set; }

		public virtual Notification Notification { get; set; }

		[Index("IDX_NotificationDelivery_ByNotificationAndTransport", 2)]
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

	public class MinCourseRoleAttribute : Attribute
	{
		public readonly CourseRole MinCourseRole;

		public MinCourseRoleAttribute(CourseRole minCourseRole)
		{
			MinCourseRole = minCourseRole;
		}
	}

	public class NotificationTypeAttribute : Attribute
	{
		public NotificationType Type { get; }

		public NotificationTypeAttribute(NotificationType type)
		{
			Type = type;
		}
	}

	public class SysAdminsOnlyAttribute : Attribute
	{
	}

	public class DefaultFrequencyAttribute : Attribute
	{
		public NotificationSendingFrequency Frequency { get; }

		public DefaultFrequencyAttribute(NotificationSendingFrequency frequency)
		{
			Frequency = frequency;
		}
	}

	public enum NotificationType : short
	{
		// Everybody
		[Display(Name = "Системное сообщение", GroupName = "Системные сообщения")]
		SystemMessage = 1,

		[Display(Name = "Сообщение от преподавателя", GroupName = "Сообщения от преподавателя")]
		[DefaultFrequency(NotificationSendingFrequency.AtOnce)]
		InstructorMessage = 2,

		[Display(Name = "Новый комментарий", GroupName = "Новые комментарии")]
		NewComment = 3,

		[Display(Name = "Ответ на ваш комментарий", GroupName = "Ответы на ваши комментарии")]
		[DefaultFrequency(NotificationSendingFrequency.AtOnce)]
		RepliedToYourComment = 4,

		[Display(Name = "Отметка «нравится» вашему комментарию", GroupName = "Отметки «нравится» вашим комментариям")]
		[DefaultFrequency(NotificationSendingFrequency.AtOnce)]
		LikedYourComment = 5,

		[Display(Name = "Прохождение код-ревью", GroupName = "Прохождение код-ревью")]
		[DefaultFrequency(NotificationSendingFrequency.AtOnce)]
		PassedManualExerciseChecking = 6,

		[Display(Name = "Тест проверен преподавателем", GroupName = "Тест проверен преподавателем")]
		[DefaultFrequency(NotificationSendingFrequency.AtOnce)]
		PassedManualQuizChecking = 7,

		[Display(Name = "Получен сертификат", GroupName = "Полученные сертификаты")]
		[DefaultFrequency(NotificationSendingFrequency.AtOnce)]
		ReceivedCertificate = 8,

		[Display(Name = "Преподаватель выставил дополнительные баллы", GroupName = "Дополнительные баллы, выставленные преподавателем")]
		[DefaultFrequency(NotificationSendingFrequency.AtOnce)]
		ReceivedAdditionalScore = 9,

		// Instructors
		[Display(Name = "Кто-то присоединился к вашей группе", GroupName = "Кто-то присоединился к вашей группе")]
		[MinCourseRole(CourseRole.Instructor)]
		[DefaultFrequency(NotificationSendingFrequency.OnceADay)]
		JoinedToYourGroup = 101,

		// Course admins
		[Display(Name = "Новый преподаватель", GroupName = "Новые преподаватели")]
		[MinCourseRole(CourseRole.CourseAdmin)]
		[DefaultFrequency(NotificationSendingFrequency.AtOnce)]
		AddedInstructor = 201,

		[Display(Name = "Новая группа", GroupName = "Новые группы")]
		[MinCourseRole(CourseRole.CourseAdmin)]
		[DefaultFrequency(NotificationSendingFrequency.AtOnce)]
		CreatedGroup = 202,

		[Display(Name = "Загружен новый пакет", GroupName = "Загружен новый пакет")]
		[MinCourseRole(CourseRole.CourseAdmin)]
		UploadedPackage = 203,

		[Display(Name = "Опубликован новый пакет", GroupName = "Опубликован новый пакет")]
		[MinCourseRole(CourseRole.CourseAdmin)]
		PublishedPackage = 204,

		[Display(Name = "Произошла ошибка", GroupName = "Произошедшие ошибки")]
		[SysAdminsOnly]
		[DefaultFrequency(NotificationSendingFrequency.AtOnce)]
		OccuredError = 301
	}

	public static class NotificationTypeExtensions
	{
		public static CourseRole GetMinCourseRole(this NotificationType type)
		{
			var attribute = type.GetAttribute<MinCourseRoleAttribute>();
			return attribute?.MinCourseRole ?? CourseRole.Student;
		}

		public static string GetDisplayName(this NotificationType type)
		{
			return type.GetAttribute<DisplayAttribute>().GetName();
		}

		public static string GetGroupName(this NotificationType type)
		{
			return type.GetAttribute<DisplayAttribute>().GetGroupName();
		}

		public static bool IsForSysAdminsOnly(this NotificationType type)
		{
			return type.GetAttribute<SysAdminsOnlyAttribute>() != null;
		}

		public static NotificationSendingFrequency GetDefaultFrequency(this NotificationType type)
		{
			var attribute = type.GetAttribute<DefaultFrequencyAttribute>();
			return attribute?.Frequency ?? NotificationSendingFrequency.Disabled;
		}
	}

	public abstract class Notification
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[StringLength(100)]
		[Required(AllowEmptyStrings = true)]
		[Index("IDX_Notification_ByCourse")]
		public string CourseId { get; set; }

		[StringLength(60)]
		[Required]
		public string InitiatedById { get; set; }

		public virtual ApplicationUser InitiatedBy { get; set; }

		[Required]
		[Index("IDX_Notification_ByCreateTime")]
		public DateTime CreateTime { get; set; }

		public virtual ICollection<NotificationDelivery> Deliveries { get; set; }

		public abstract string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course);
		public abstract string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course);
	}

	public static class NotificationExtensions
	{
		public static NotificationType GetNotificationType(this Type notificationClass)
		{
			if (!notificationClass.IsSubclassOf(typeof(Notification)))
				throw new ArgumentException(@"Should be subclass of Notification class", nameof(notificationClass));

			var attribute = (NotificationTypeAttribute)notificationClass.GetCustomAttribute(typeof(NotificationTypeAttribute), false);
			return attribute.Type;
		}

		public static NotificationType GetNotificationType(this Notification notification)
		{
			return GetNotificationType(ObjectContext.GetObjectType(notification.GetType()));
		}
	}

	[NotificationType(NotificationType.SystemMessage)]
	public class SystemMessageNotification : Notification
	{
		[Required]
		public string Text { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			return $"<b>Сообщение от ulearn.me:</b><br/><br/>{Text.EscapeHtml()}";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			return $"Сообщение от ulearn.me:\n\n{Text}";
		}
	}

	[NotificationType(NotificationType.InstructorMessage)]
	public class InstructorMessageNotification : Notification
	{
		[Required]
		public string Text { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			return $"<b>Сообщение от преподавателя:</b><br/><br/>{Text.EscapeHtml()}";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			return $"Сообщение от преподавателя:\n\n{Text}";
		}
	}

	public abstract class AbstractCommentNotification : Notification
	{
		[Required]
		public int CommentId { get; set; }

		public virtual Comment Comment { get; set; }

		protected string GetSlideTitle(Course course, Slide slide)
		{
			return $"{course.Title.MakeNestedQuotes()}: {slide.Title.MakeNestedQuotes()}";
		}
		
		protected string GetCommentUrl(Slide slide)
		{
			/* TODO (andgein): Build url from UrlHelper */
			return "https://ulearn.me/Course/" + Comment.CourseId + "/" + slide.Url + "#comment-" + Comment.Id;
		}
	}

	[NotificationType(NotificationType.NewComment)]
	public class NewCommentNotification : AbstractCommentNotification
	{
		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			var slide = course.FindSlideById(Comment.SlideId);
			if (slide == null)
				return null;

			return $"<b>{Comment.Author.VisibleName.EscapeHtml()} в «{GetSlideTitle(course, slide).EscapeHtml()}»</b><br/><br/>{Comment.Text.Trim().EscapeHtml()}<br/><br/>{GetCommentUrl(slide).EscapeHtml()}";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			var slide = course.FindSlideById(Comment.SlideId);
			if (slide == null)
				return null;

			return $"{Comment.Author.VisibleName} в «{GetSlideTitle(course, slide)}»\n\n{Comment.Text.Trim()}\n\n{GetCommentUrl(slide)}";
		}
	}

	[NotificationType(NotificationType.RepliedToYourComment)]
	public class RepliedToYourCommentNotification : AbstractCommentNotification
	{
		[Required]
		public int ParentCommentId { get; set; }

		public virtual Comment ParentComment { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			var slide = course.FindSlideById(Comment.SlideId);
			if (slide == null)
				return null;

			return $"<b>{Comment.Author.VisibleName.EscapeHtml()} ответил(а) на ваш комментарий в «{GetSlideTitle(course, slide).EscapeHtml()}»</b><br/><br/>" + 
				   $"<i>{ParentComment.Text.Trim().EscapeHtml()}</i><br>" +	
				   $"{Comment.Text.Trim().EscapeHtml()}<br/><br/>" + 
				   $"{GetCommentUrl(slide).EscapeHtml()}";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			var slide = course.FindSlideById(Comment.SlideId);
			if (slide == null)
				return null;

			return $"{Comment.Author.VisibleName} ответил(а) на ваш комментарий в «{GetSlideTitle(course, slide)}»\n\n" +
				   $"> {ParentComment.Text.Trim()}\n" +
				   $"{Comment.Text.Trim()}\n\n" +
				   $"{GetCommentUrl(slide)}";
		}
	}

	[NotificationType(NotificationType.LikedYourComment)]
	public class LikedYourCommentNotification : AbstractCommentNotification
	{
		[Required]
		[StringLength(64)]
		public string LikedUserId { get; set; }

		public virtual ApplicationUser LikedUser { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			var slide = course.FindSlideById(Comment.SlideId);
			if (slide == null)
				return null;

			return $"<b>{InitiatedBy.VisibleName.EscapeHtml()} оценил(а) ваш комментарий в «{GetSlideTitle(course, slide).EscapeHtml()}»</b><br/><br/>" +
				   $"<i>{Comment.Text.Trim().EscapeHtml()}</i><br/><br/>" +
				   $"{GetCommentUrl(slide).EscapeHtml()}";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			var slide = course.FindSlideById(Comment.SlideId);
			if (slide == null)
				return null;


			return $"{InitiatedBy.VisibleName} оценил(а) ваш комментарий в «{GetSlideTitle(course, slide)}»\n\n" +
				   $"> {Comment.Text.Trim()}\n\n" +
				   $"{GetCommentUrl(slide)}";
		}
	}

	[NotificationType(NotificationType.PassedManualExerciseChecking)]
	public class PassedManualExerciseCheckingNotification : Notification
	{
		[Required]
		public int CheckingId { get; set; }

		public virtual ManualExerciseChecking Checking { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			throw new NotImplementedException();
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			throw new NotImplementedException();
		}
	}

	[NotificationType(NotificationType.PassedManualQuizChecking)]
	public class PassedManualQuizCheckingNotification : Notification
	{
		[Required]
		public int CheckingId { get; set; }

		public virtual ManualQuizChecking Checking { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			throw new NotImplementedException();
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			throw new NotImplementedException();
		}
	}

	[NotificationType(NotificationType.ReceivedCertificate)]
	public class ReceivedCertificateNotification : Notification
	{
		[Required]
		public Guid CertificateId { get; set; }

		public virtual Certificate Certificate { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			throw new NotImplementedException();
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			throw new NotImplementedException();
		}
	}

	[NotificationType(NotificationType.ReceivedAdditionalScore)]
	public class ReceivedAdditionalScoreNotification : Notification
	{
		[Required]
		public int ScoreId { get; set; }

		public virtual AdditionalScore Score { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			throw new NotImplementedException();
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			throw new NotImplementedException();
		}
	}

	[NotificationType(NotificationType.JoinedToYourGroup)]
	public class JoinedToYourGroupNotification : Notification
	{
		[Required]
		public int GroupId { get; set; }

		public virtual Group Group { get; set; }

		[Required]
		[StringLength(64)]
		public string JoinedUserId { get; set; }

		public virtual ApplicationUser JoinedUser { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			throw new NotImplementedException();
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			throw new NotImplementedException();
		}
	}

	[NotificationType(NotificationType.AddedInstructor)]
	public class AddedInstructorNotification : Notification
	{
		[Required]
		[StringLength(64)]
		public string AddedUserId { get; set; }

		public virtual ApplicationUser AddedUser { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			throw new NotImplementedException();
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			throw new NotImplementedException();
		}
	}

	[NotificationType(NotificationType.CreatedGroup)]
	public class CreatedGroupNotification : Notification
	{
		[Required]
		public int GroupId { get; set; }

		public virtual Group Group { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			throw new NotImplementedException();
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			throw new NotImplementedException();
		}
	}

	[NotificationType(NotificationType.UploadedPackage)]
	public class UploadedPackageNotification : Notification
	{
		[Required]
		public int CourseVersionId { get; set; }

		public virtual CourseVersion CourseVersion { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			throw new NotImplementedException();
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			throw new NotImplementedException();
		}
	}

	[NotificationType(NotificationType.PublishedPackage)]
	public class PublishedPackageNotification : Notification
	{
		[Required]
		public int CourseVersionId { get; set; }

		public virtual CourseVersion CourseVersion { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			throw new NotImplementedException();
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			throw new NotImplementedException();
		}
	}

	[NotificationType(NotificationType.OccuredError)]
	public class OccuredErrorNotification : Notification
	{
		[Required]
		[StringLength(100)]
		public string ErrorId { get; set; }

		public string ErrorMessage { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course)
		{
			throw new NotImplementedException();
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course)
		{
			throw new NotImplementedException();
		}
	}
}