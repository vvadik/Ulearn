using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Common.Api.Models.Responses;

namespace Ulearn.Web.Api.Models.Responses.User
{
	[DataContract]
	public class UsersProgressResponse : SuccessResponse
	{
		[DataMember]
		public List<UserProgress> SlidesWithScore { get; set; }
	}

	public class UserProgress
	{
		public string UserId { get; set; }
		public Dictionary<Guid, UserProgressSlideResult> SlidesWithScore { get; set; }
	}

	public class UserProgressSlideResult
	{
		public int Score { get; set; }
	}
}