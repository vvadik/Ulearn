using System;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Results.Notifications
{
	[DataContract]
	public class NotificationsCountResult
	{
		[DataMember(Name = "count")]
		public int Count { get; set; }

		[DataMember(Name = "last_timestamp")]
		public DateTime? LastTimestamp { get; set; }
	}
}