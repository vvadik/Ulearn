using System.Collections.Generic;
using System.Runtime.Serialization;
using Database.Models;

namespace Ulearn.Web.Api.Models.Responses.Account
{
	[DataContract]
	public class CourseRolesResponse : ApiResponse
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
		public CourseRole Role { get; set; }
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