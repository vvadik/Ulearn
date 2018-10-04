using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Database.Models;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Groups
{
	[DataContract]
	public class GroupInstructorsResponse
	{
		[DataMember(Name = "instructors")]
		public List<GroupInstructorInfo> Instructors { get; set; } 
	}

	[DataContract]
	public class GroupInstructorInfo
	{
		[DataMember(Name = "user")]
		public ShortUserInfo User { get; set; }

		[DataMember(Name = "access_type")]
		public GroupAccessType AccessType { get; set; }
		
		[DataMember(Name = "granted_by")]
		public ShortUserInfo GrantedBy { get; set; }
		
		[DataMember(Name = "grant_time")]
		public DateTime GrantTime { get; set; }
	}
}