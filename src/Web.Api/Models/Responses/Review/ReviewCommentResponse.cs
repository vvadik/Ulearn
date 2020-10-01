using System;
using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Review
{
	[DataContract]
	public class ReviewCommentResponse
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int ReviewId { get; set; }

		[DataMember]
		public string Text { get; set; }
		
		[DataMember]
		public DateTime PublishTime { get; set; }

		[DataMember]
		public ShortUserInfo Author { get; set; }
	}
}