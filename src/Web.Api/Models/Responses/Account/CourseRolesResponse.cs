using System.Collections.Generic;
using System.Runtime.Serialization;
using Database.Models;
using Ulearn.Common.Api.Models.Responses;

namespace Ulearn.Web.Api.Models.Responses.Account
{
	[DataContract]
	public class CourseRolesResponse : SuccessResponse
	{
		[DataMember(Name = "is_system_administrator")]
		public bool IsSystemAdministrator { get; set; }
		
		[DataMember(Name = "course_roles")]
		public List<CourseRoleResponse> Roles { get; set; }

		[DataMember(Name = "course_accesses")]
		public List<CourseAccessResponse> Accesses { get; set; }
	}

	[DataContract]
	public class CourseRoleResponse
	{
		[DataMember(Name = "course_id")]
		public string CourseId { get; set; }
		
		[DataMember(Name = "role")]
		public CourseRoleType Role { get; set; }
	}

	[DataContract]
	public class CourseAccessResponse
	{
		[DataMember(Name = "course_id")]
		public string CourseId { get; set; }
		
		[DataMember(Name = "accesses")]
		public List<CourseAccessType> Accesses { get; set; }
	}
}