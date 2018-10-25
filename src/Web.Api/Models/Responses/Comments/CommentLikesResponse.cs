using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Comments
{
	[DataContract]
	public class CommentLikesResponse : PaginatedResponse
	{
		[DataMember(Name = "likes")]
		public List<CommentLikeInfo> Likes { get; set; }
	}

	[DataContract]
	public class CommentLikeInfo
	{
		[DataMember(Name = "user")]
		public ShortUserInfo User { get; set; }
		
		[DataMember(Name = "timestamp")]
		public DateTime Timestamp { get; set; }
	}
}