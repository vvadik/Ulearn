using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.User
{
	[DataContract]
	public class UserProgressResponse : SuccessResponse
	{
		[DataMember]
		public Dictionary<Guid, UserSlideResult> VisitedSlides { get; set; }
	}

	public class UserSlideResult
	{
		public int Score { get; set; }
		public int UsedAttempts { get; set; }
		public bool IsWaitingForManualChecking { get; set; }
		public bool Visited { get; set; }
	}
}