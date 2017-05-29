using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects;
using System.Reflection;
using Database.DataContexts;
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
		[Index("IDX_NotificationTransport_ByUserAndDeleted", 1)]
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

		public bool IsEnabled { get; set; }

		[Index("IDX_NotificationTransport_ByUserAndDeleted", 2)]
		public bool IsDeleted { get; set; }
	}

	public class MailNotificationTransport : NotificationTransport
	{
		public override string ToString()
		{
			return $"Email <{User.Email ?? User.Id}> (#{Id})";
		}
	}

	public class TelegramNotificationTransport : NotificationTransport
	{
		public override string ToString()
		{
			return $"Telegram <{User.TelegramChatId?.ToString() ?? User.Id}> (#{Id})";
		}
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

		public bool IsEnabled { get; set; }
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

		[Index("IDX_NotificationDelivery_ByNextTryTime")]
		public DateTime? NextTryTime { get; set; }

		public int FailsCount { get; set; }
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

	public class IsEnabledByDefaultAttribute : Attribute
	{
		public bool IsEnabled { get; }

		public IsEnabledByDefaultAttribute(bool isEnabled)
		{
			IsEnabled = isEnabled;
		}
	}

	public enum NotificationType : short
	{
		// Everybody
		[Display(Name = @"Сообщение от платформы ulearn.me", GroupName = @"Сообщения от платформы ulearn.me")]
		SystemMessage = 1,

		[Display(Name = @"Сообщение от преподавателя", GroupName = @"Сообщения от преподавателя")]
		[IsEnabledByDefault(true)]
		InstructorMessage = 2,

		[Display(Name = @"Новый комментарий", GroupName = @"Новые комментарии")]
		NewComment = 3,

		[Display(Name = @"Ответ на ваш комментарий", GroupName = @"Ответы на ваши комментарии")]
		[IsEnabledByDefault(true)]
		RepliedToYourComment = 4,

		[Display(Name = @"Отметка «нравится» вашему комментарию", GroupName = @"Отметки «нравится» вашим комментариям")]
		[IsEnabledByDefault(true)]
		LikedYourComment = 5,

		[Display(Name = @"Вы прошли код-ревью", GroupName = @"Вы прошли код-ревью")]
		[IsEnabledByDefault(true)]
		PassedManualExerciseChecking = 6,

		[Display(Name = @"Тест проверен преподавателем", GroupName = @"Тест проверен преподавателем")]
		[IsEnabledByDefault(true)]
		PassedManualQuizChecking = 7,

		[Display(Name = @"Вы получили сертификат", GroupName = @"Вы получили сертификаты")]
		[IsEnabledByDefault(true)]
		ReceivedCertificate = 8,

		[Display(Name = @"Преподаватель выставил дополнительные баллы", GroupName = @"Дополнительные баллы, выставленные преподавателем")]
		[IsEnabledByDefault(true)]
		ReceivedAdditionalScore = 9,

		// Instructors
		[Display(Name = @"Кто-то присоединился к вашей группе", GroupName = @"Кто-то присоединился к вашей группе")]
		[MinCourseRole(CourseRole.Instructor)]
		[IsEnabledByDefault(true)]
		JoinedToYourGroup = 101,

		// Course admins
		[Display(Name = @"Добавлен новый преподаватель", GroupName = @"Добавлены новые преподаватели")]
		[MinCourseRole(CourseRole.CourseAdmin)]
		[IsEnabledByDefault(true)]
		AddedInstructor = 201,

		[Display(Name = @"Создана новая группа", GroupName = @"Созданы новые группы")]
		[MinCourseRole(CourseRole.CourseAdmin)]
		[IsEnabledByDefault(true)]
		CreatedGroup = 202,

		[Display(Name = @"Загружен новый пакет", GroupName = @"Загружены новые пакеты")]
		[MinCourseRole(CourseRole.CourseAdmin)]
		UploadedPackage = 203,

		[Display(Name = @"Опубликован новый пакет", GroupName = @"Опубликованы новые пакеты")]
		[MinCourseRole(CourseRole.CourseAdmin)]
		PublishedPackage = 204,

		[Display(Name = @"Произошла ошибка", GroupName = @"Произошли ошибки")]
		[SysAdminsOnly]
		[IsEnabledByDefault(true)]
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

		public static bool IsEnabledByDefault(this NotificationType type)
		{
			var attribute = type.GetAttribute<IsEnabledByDefaultAttribute>();
			return attribute?.IsEnabled ?? false;
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

		[Required]
		[Index("IDX_Notification_ByAreDeliveriesCreated")]
		public bool AreDeliveriesCreated { get; set; }

		public virtual ICollection<NotificationDelivery> Deliveries { get; set; }

		public abstract string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl);
		public abstract string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl);

		public abstract List<string> GetRecipientsIds(ULearnDb db);

		public abstract bool IsActual();

		protected string GetSlideTitle(Course course, Slide slide)
		{
			return $"{course.Title.MakeNestedQuotes()}: {slide.Title.MakeNestedQuotes()}";
		}

		protected string GetUnitTitle(Course course, Unit unit)
		{
			return $"{course.Title.MakeNestedQuotes()}: {unit.Title.MakeNestedQuotes()}";
		}

		protected string GetSlideUrl(Course course, Slide slide, string baseUrl)
		{
			return baseUrl + $"/Course/{course.Id}/{slide.Url}";
		}

		protected string GetCourseUrl(Course course, string baseUrl)
		{
			return baseUrl + $"/Course/{course.Id}/";
		}
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

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			return $"<b>Сообщение от ulearn.me:</b><br/><br/>{Text.EscapeHtml()}";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			return $"Сообщение от ulearn.me:\n\n{Text}";
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			/* If you want to send system message you should create NotificationDelivery yourself. By default nobody receives it */
			return new List<string>();
		}

		public override bool IsActual()
		{
			return true;
		}
	}

	[NotificationType(NotificationType.InstructorMessage)]
	public class InstructorMessageNotification : Notification
	{
		[Required]
		public string Text { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			return $"<b>Сообщение от преподавателя:</b><br/><br/>{Text.EscapeHtml()}";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			return $"Сообщение от преподавателя:\n\n{Text}";
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			/* If you want to send message you should create NotificationDelivery yourself. By default nobody receives it */
			return new List<string>();
		}

		public override bool IsActual()
		{
			return true;
		}
	}

	public abstract class AbstractCommentNotification : Notification
	{
		[Required]
		public int CommentId { get; set; }

		public virtual Comment Comment { get; set; }
		
		protected string GetCommentUrl(Course course, Slide slide, string baseUrl)
		{
			return GetSlideUrl(course, slide, baseUrl) + "#comment-" + Comment.Id;
		}

		public override bool IsActual()
		{
			return ! Comment.IsDeleted && Comment.IsApproved;
		}
	}

	[NotificationType(NotificationType.NewComment)]
	public class NewCommentNotification : AbstractCommentNotification
	{
		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			var slide = course.FindSlideById(Comment.SlideId);
			if (slide == null)
				return null;

			return $"<b>{Comment.Author.VisibleName.EscapeHtml()} в «{GetSlideTitle(course, slide).EscapeHtml()}»</b><br/><br/>{Comment.Text.Trim().EscapeHtml()}<br/><br/>{GetCommentUrl(course, slide, baseUrl).EscapeHtml()}";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			var slide = course.FindSlideById(Comment.SlideId);
			if (slide == null)
				return null;

			return $"{Comment.Author.VisibleName} в «{GetSlideTitle(course, slide)}»\n\n{Comment.Text.Trim()}\n\n{GetCommentUrl(course, slide, baseUrl)}";
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			return new VisitsRepo(db).GetCourseUsers(CourseId);
		}
	}

	[NotificationType(NotificationType.RepliedToYourComment)]
	public class RepliedToYourCommentNotification : AbstractCommentNotification
	{
		[Required]
		public int ParentCommentId { get; set; }

		public virtual Comment ParentComment { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			var slide = course.FindSlideById(Comment.SlideId);
			if (slide == null)
				return null;

			return $"<b>{Comment.Author.VisibleName.EscapeHtml()} ответил(а) на ваш комментарий в «{GetSlideTitle(course, slide).EscapeHtml()}»</b><br/><br/>" + 
				   $"<i>{ParentComment.Text.Trim().EscapeHtml()}</i><br>" +	
				   $"{Comment.Text.Trim().EscapeHtml()}<br/><br/>" + 
				   $"{GetCommentUrl(course, slide, baseUrl).EscapeHtml()}";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			var slide = course.FindSlideById(Comment.SlideId);
			if (slide == null)
				return null;

			return $"{Comment.Author.VisibleName} ответил(а) на ваш комментарий в «{GetSlideTitle(course, slide)}»\n\n" +
				   $"> {ParentComment.Text.Trim()}\n" +
				   $"{Comment.Text.Trim()}\n\n" +
				   $"{GetCommentUrl(course, slide, baseUrl)}";
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			return new List<string> { ParentComment.AuthorId };
		}
	}

	[NotificationType(NotificationType.LikedYourComment)]
	public class LikedYourCommentNotification : AbstractCommentNotification
	{
		[Required]
		[StringLength(64)]
		public string LikedUserId { get; set; }

		public virtual ApplicationUser LikedUser { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			var slide = course.FindSlideById(Comment.SlideId);
			if (slide == null)
				return null;

			return $"<b>{InitiatedBy.VisibleName.EscapeHtml()} оценил(а) ваш комментарий в «{GetSlideTitle(course, slide).EscapeHtml()}»</b><br/><br/>" +
				   $"<i>{Comment.Text.Trim().EscapeHtml()}</i><br/><br/>" +
				   $"{GetCommentUrl(course, slide, baseUrl).EscapeHtml()}";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			var slide = course.FindSlideById(Comment.SlideId);
			if (slide == null)
				return null;
			
			return $"{InitiatedBy.VisibleName} оценил(а) ваш комментарий в «{GetSlideTitle(course, slide)}»\n\n" +
				   $"> {Comment.Text.Trim()}\n\n" +
				   $"{GetCommentUrl(course, slide, baseUrl)}";
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			return new List<string> { Comment.AuthorId };
		}
	}

	[NotificationType(NotificationType.PassedManualExerciseChecking)]
	public class PassedManualExerciseCheckingNotification : Notification
	{
		[Required]
		public int CheckingId { get; set; }

		public virtual ManualExerciseChecking Checking { get; set; }

		private string GetReviewsText(bool html)
		{
			var commentsText = "";
			if (Checking.NotDeletedReviews.Count > 0)
			{
				commentsText = html ? "<b>Комментарии:</b><br>" : "Комментарии:";
				foreach (var review in Checking.NotDeletedReviews)
				{
					if (review.StartLine == review.FinishLine)
						commentsText += $"Строка {review.StartLine}: ";
					else
						commentsText += $"Строки {review.StartLine}—{review.FinishLine}: ";

					commentsText += html ? $"<i>{review.Comment.EscapeHtml()}</i><br>" : $"{review.Comment}";
				}
			}
			return commentsText;
		}

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			var slide = course.FindSlideById(Checking.SlideId);
			if (slide == null)
				return null;

			var commentsText = GetReviewsText(html: true);

			return $"<b>{InitiatedBy.VisibleName.EscapeHtml()} проверил(а) ваше решение в «{GetSlideTitle(course, slide).EscapeHtml()}»:</b><br/>" +
				   $"вы получили {Checking.Score.PluralizeInRussian(RussianPluralizationOptions.Score)}<br/><br/>" +
				   commentsText + "<br/>" +
				   $"{GetSlideUrl(course, slide, baseUrl).EscapeHtml()}";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			var slide = course.FindSlideById(Checking.SlideId);
			if (slide == null)
				return null;

			var commentsText = GetReviewsText(html: false);

			return $"{InitiatedBy.VisibleName} проверил(а) ваше решение в «{GetSlideTitle(course, slide)}»:\n" +
				   $"вы получили {Checking.Score.PluralizeInRussian(RussianPluralizationOptions.Score)}\n\n" +
				   commentsText + "\n" +
				   $"{GetSlideUrl(course, slide, baseUrl)}";
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			return new List<string> { Checking.UserId };
		}

		public override bool IsActual()
		{
			return true;
		}
	}

	[NotificationType(NotificationType.PassedManualQuizChecking)]
	public class PassedManualQuizCheckingNotification : Notification
	{
		[Required]
		public int CheckingId { get; set; }

		public virtual ManualQuizChecking Checking { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			var slide = course.FindSlideById(Checking.SlideId);
			if (slide == null)
				return null;

			return $"<b>{InitiatedBy.VisibleName.EscapeHtml()} проверил(а) ваш тест «{GetSlideTitle(course, slide).EscapeHtml()}»:</b><br/>" +
				   $"вы получили {Checking.Score.PluralizeInRussian(RussianPluralizationOptions.Score)}<br/><br/>" +
				   $"{GetSlideUrl(course, slide, baseUrl).EscapeHtml()}";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			var slide = course.FindSlideById(Checking.SlideId);
			if (slide == null)
				return null;

			return $"{InitiatedBy.VisibleName} проверил(а) ваш тест «{GetSlideTitle(course, slide)}»:\n" +
				   $"вы получили {Checking.Score.PluralizeInRussian(RussianPluralizationOptions.Score)}\n\n" +
				   $"{GetSlideUrl(course, slide, baseUrl)}";
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			return new List<string> { Checking.UserId };
		}

		public override bool IsActual()
		{
			return true;
		}
	}

	[NotificationType(NotificationType.ReceivedCertificate)]
	public class ReceivedCertificateNotification : Notification
	{
		[Required]
		public Guid CertificateId { get; set; }

		public virtual Certificate Certificate { get; set; }

		private static string GetCertificateUrl(Certificate certificate, string baseUrl)
		{
			return baseUrl + $"/Certificates/{certificate.Id}";
		}

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			return $"<b>Поздравляем! Вы получили сертификат по курсу «{course.Title.EscapeHtml()}».</b><br/><br/>" +
				   $"Посмотреть сертификат можно по ссылке {GetCertificateUrl(Certificate, baseUrl).EscapeHtml()} или в любой момент на ulearn.me.<br/><br/>" +
				   "Поделитесь ссылкой на сертификат с друзьями в социальных сетях — пусть ваше достижение увидят все!";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			return $"Поздравляем! Вы получили сертификат по курсу «{course.Title}».\n\n" +
				   $"Посмотреть сертификат можно по ссылке {GetCertificateUrl(Certificate, baseUrl)} или в любой момент на ulearn.me.\n\n" +
				   "Поделитесь ссылкой на сертификат с друзьями в социальных сетях — пусть ваше достижение увидят все!";
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			return new List<string> { Certificate.UserId };
		}

		public override bool IsActual()
		{
			return !Certificate.IsDeleted;
		}
	}

	[NotificationType(NotificationType.ReceivedAdditionalScore)]
	public class ReceivedAdditionalScoreNotification : Notification
	{
		[Required]
		public int ScoreId { get; set; }

		public virtual AdditionalScore Score { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			var unit = course.FindUnitById(Score.UnitId);
			if (unit == null)
				return null;

			return $"<b>{InitiatedBy.VisibleName.EscapeHtml()} поставил(а) вам баллы за модуль «{GetUnitTitle(course, unit).EscapeHtml()}»:</b><br/>" +
				   $"вы получили {Score.Score.PluralizeInRussian(RussianPluralizationOptions.Score)}<br/><br/>" +
				   "Полную ведомость смотрите на ulearn.me.";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			var unit = course.FindUnitById(Score.UnitId);
			if (unit == null)
				return null;

			return $"{InitiatedBy.VisibleName} поставил(а) вам баллы за модуль «{GetUnitTitle(course, unit)}»:\n" +
				   $"вы получили {Score.Score.PluralizeInRussian(RussianPluralizationOptions.Score)}\n\n" +
				   "Полную ведомость смотрите на ulearn.me.";
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			return new List<string> { Score.UserId };
		}

		public override bool IsActual()
		{
			return true;
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

		private static string GetGroupsUrl(Course course, string baseUrl)
		{
			return baseUrl + $"/Admin/Groups?courseId={course.Id.EscapeHtml()}";
		}

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			return $"<b>{JoinedUser.VisibleName.EscapeHtml()}</b> присоединился(лась) к вашей группе «{Group.Name.EscapeHtml()}» по курсу «{course.Title.EscapeHtml()}».<br/><br/>" +
				   $"Посмотреть участников группы можно по ссылке {GetGroupsUrl(course, baseUrl).EscapeHtml()}";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			return $"{JoinedUser.VisibleName} присоединился(лась) к вашей группе «{Group.Name}» по курсу «{course.Title}».\n\n" +
				   $"Посмотреть участников группы можно по ссылке {GetGroupsUrl(course, baseUrl)}";
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			return new List<string> { Group.OwnerId };
		}

		public override bool IsActual()
		{
			return !Group.IsDeleted;
		}
	}

	[NotificationType(NotificationType.AddedInstructor)]
	public class AddedInstructorNotification : Notification
	{
		[Required]
		[StringLength(64)]
		public string AddedUserId { get; set; }

		public virtual ApplicationUser AddedUser { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			return $"<b>{AddedUser.VisibleName.EscapeHtml()}</b> стал(а) преподавателем курса «{course.Title.EscapeHtml()}».";
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			return $"{AddedUser.VisibleName} стал(а) преподавателем курса «{course.Title}».";
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			return new UserRolesRepo(db).GetListOfUsersWithCourseRole(CourseRole.CourseAdmin, CourseId);
		}

		public override bool IsActual()
		{
			return true;
		}
	}

	[NotificationType(NotificationType.CreatedGroup)]
	public class CreatedGroupNotification : Notification
	{
		[Required]
		public int GroupId { get; set; }

		public virtual Group Group { get; set; }

		protected static string GetGroupsUrl(Course course, string baseUrl)
		{
			return baseUrl + $"/Admin/Groups?courseId={course.Id.EscapeHtml()}";
		}

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			return $"<b>{Group.Owner.VisibleName.EscapeHtml()}</b> создал(а) новую группу <b>«{Group.Name.EscapeHtml()}»</b> в курсе «{course.Title.EscapeHtml()}».<br/><br/>" +
				   GetGroupsUrl(course, baseUrl);
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			return $"{Group.Owner.VisibleName} создал(а) новую группу «{Group.Name}» в курсе «{course.Title}».\n\n" +
				   GetGroupsUrl(course, baseUrl);
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			return new UserRolesRepo(db).GetListOfUsersWithCourseRole(CourseRole.CourseAdmin, CourseId);
		}

		public override bool IsActual()
		{
			return !Group.IsDeleted;
		}
	}

	public abstract class AbstractPackageNotification : Notification
	{
		protected static string GetPackagesUrl(Course course, string baseUrl)
		{
			return baseUrl + $"/Admin/Packages?courseId={course.Id.EscapeHtml()}";
		}
	}

	[NotificationType(NotificationType.UploadedPackage)]
	public class UploadedPackageNotification : AbstractPackageNotification
	{
		[Required]
		public Guid CourseVersionId { get; set; }

		public virtual CourseVersion CourseVersion { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			return $"Загружена новая версия курса <b>«{course.Title.EscapeHtml()}»</b>. Теперь её можно опубликовать.<br/><br/>" +
				   GetPackagesUrl(course, baseUrl);
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			return $"Загружена новая версия курса «{course.Title.EscapeHtml()}». Теперь её можно опубликовать.\n\n" +
				   GetPackagesUrl(course, baseUrl);
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			return new UserRolesRepo(db).GetListOfUsersWithCourseRole(CourseRole.CourseAdmin, CourseId);
		}

		public override bool IsActual()
		{
			return true;
		}
	}

	[NotificationType(NotificationType.PublishedPackage)]
	public class PublishedPackageNotification : AbstractPackageNotification
	{
		[Required]
		public Guid CourseVersionId { get; set; }

		public virtual CourseVersion CourseVersion { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			return $"Опубликована новая версия курса <b>«{course.Title.EscapeHtml()}»</b>.<br/><br/>" +
				   GetCourseUrl(course, baseUrl).EscapeHtml() + "<br/><br/>" + 
				   "Предыдущие версии:<br>" + GetPackagesUrl(course, baseUrl).EscapeHtml();
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			return $"Опубликована новая версия курса «{course.Title.EscapeHtml()}».\n\n" +
				   GetCourseUrl(course, baseUrl) + "\n\n" +
				   "Предыдущие версии:\n" + GetPackagesUrl(course, baseUrl);
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			return new UserRolesRepo(db).GetListOfUsersWithCourseRole(CourseRole.CourseAdmin, CourseId);
		}

		public override bool IsActual()
		{
			return true;
		}
	}

	[NotificationType(NotificationType.OccuredError)]
	public class OccuredErrorNotification : Notification
	{
		[Required]
		[StringLength(100)]
		public string ErrorId { get; set; }

		public string ErrorMessage { get; set; }

		public override string GetHtmlMessageForDelivery(NotificationTransport transport, NotificationDelivery delivery, Course course, string baseUrl)
		{
			throw new NotImplementedException();
		}

		public override string GetTextMessageForDelivery(NotificationTransport transport, NotificationDelivery notificationDelivery, Course course, string baseUrl)
		{
			throw new NotImplementedException();
		}

		public override List<string> GetRecipientsIds(ULearnDb db)
		{
			var userManager = new ULearnUserManager(db);
			return new UsersRepo(db).GetSysAdminsIds(userManager);
		}

		public override bool IsActual()
		{
			return true;
		}
	}
}