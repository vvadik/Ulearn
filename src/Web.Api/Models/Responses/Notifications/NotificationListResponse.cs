using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Notifications
{
	[DataContract]
	public class NotificationListResponse : SuccessResponse
	{
		[DataMember(Name = "important")]
		public NotificationList Important { get; set; }
		
		[DataMember(Name = "comments")]
		public NotificationList Comments { get; set; }
	}

	[DataContract]
	public class NotificationList
	{
		[DataMember(Name = "last_view_timestamp")]
		public DateTime? LastViewTimestamp { get; set; }
		
		[DataMember(Name = "notifications")]
		public List<NotificationInfo> Notifications { get; set; }
	}

	[DataContract]
	public class NotificationInfo
	{
		[DataMember(Name = "id")]
		public int Id { get; set; }
		
		[DataMember(Name = "type")]
		public string Type { get; set; }
		
		[DataMember(Name = "author")]
		public ShortUserInfo Author { get; set; }
		
		[DataMember(Name = "create_time")]
		public DateTime CreateTime { get; set; }
		
		[DataMember(Name = "course_id")]
		public string CourseId { get; set; }
		
		[DataMember(Name = "data")]
		public NotificationData Data { get; set; }
	}

	[DataContract]
	public class NotificationData
	{
		[DataMember(Name = "comment", EmitDefaultValue = false)]
		public NotificationCommentInfo Comment { get; set; } 
	}

	[DataContract]
	public class NotificationCommentInfo
	{
		[DataMember(Name = "id")]
		public int Id { get; set; }
		
		[DataMember(Name = "author")]
		public ShortUserInfo Author { get; set; }
		
		[DataMember(Name = "course_id")]
		public string CourseId { get; set; }
		
		[DataMember(Name = "slide_id")]
		public Guid SlideId { get; set; }
		
		[DataMember(Name = "text")]
		public string Text { get; set; }
		
		[DataMember(Name = "publish_time")]
		public DateTime PublishTime { get; set; }
	}
}