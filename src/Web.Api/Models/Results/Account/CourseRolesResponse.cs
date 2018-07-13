using System.Collections.Generic;
using System.Runtime.Serialization;
using Database.Models;

namespace Ulearn.Web.Api.Models.Results.Account
{
	[DataContract]
	public class CourseRolesResponse
	{
		[DataMember(Name = "is_system_administrator")]
		public bool IsSystemAdministrator { get; set; }
		
		[DataMember(Name = "course_roles")]
		public List<CourseRoleResponse> Roles { get; set; }
	}

	[DataContract]
	public class CourseRoleResponse
	{
		[DataMember(Name = "course_id")]
		public string CourseId { get; set; }
		
		[DataMember(Name = "role")]
		public CourseRole Role { get; set; }
	}
}