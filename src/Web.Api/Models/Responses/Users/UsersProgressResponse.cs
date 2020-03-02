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
		/* Dictionary<unitId, Dictionary<slideId, slideProgress>> */
		public Dictionary<string, UserProgress> UsersProgress { get; set; }
	}


	[DataContract]
	public class UserProgress
	{
		[DataMember]
		public Dictionary<Guid, UserProgressSlideResult> SlidesWithScore { get; set; }
		
		[DataMember]
		public Dictionary<Guid, Dictionary<string, int>> AdditionalScores { get; set; }
	}

	[DataContract]
	public class UserProgressSlideResult
	{
		[DataMember]
		public int Score { get; set; }
	}
}