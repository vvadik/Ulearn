using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Groups
{
	[DataContract]
	public class GroupStudentsResponse : SuccessResponse
	{
		[DataMember(Name = "students")]
		public List<GroupStudentInfo> Students { get; set; }
	}

	[DataContract]
	public class GroupStudentInfo
	{
		[DataMember(Name = "user")]
		public ShortUserInfo User { get; set; }
		
		[DataMember(Name = "adding_time")]
		public DateTime? AddingTime { get; set; }
	}
}