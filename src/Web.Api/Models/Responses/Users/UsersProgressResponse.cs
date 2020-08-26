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
		/* Dictionary<userId, Dictionary<slideId, slideProgress>> */
		public Dictionary<string, UserProgress> UserProgress { get; set; }
	}
	
	[DataContract]
	public class UserProgress
	{
		[DataMember]
		public Dictionary<Guid, UserProgressSlideResult> VisitedSlides { get; set; }
		
		[DataMember]
		public Dictionary<Guid, Dictionary<string, int>> AdditionalScores { get; set; }
	}

	public class UserProgressSlideResult
	{
		public int Score { get; set; }
		public int UsedAttempts { get; set; }
		public bool IsWaitingForManualChecking { get; set; }
		public bool Visited { get; set; }
	}
}