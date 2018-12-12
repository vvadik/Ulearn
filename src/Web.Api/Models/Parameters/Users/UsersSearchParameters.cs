using System.ComponentModel.DataAnnotations;
using Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ulearn.Web.Api.Models.Parameters.Users
{
	public class UsersSearchParameters : ApiParameters
	{
		[FromQuery(Name = "query")]
		[MaxLength(100, ErrorMessage = "Query should be at most 100 chars")]
		public string Query { get; set; }
		
		[FromQuery(Name = "course_id")]
		public string CourseId { get; set; }
		
		[FromQuery(Name = "course_role")]
		public CourseRoleType? CourseRoleType { get; set; }
		
		[FromQuery(Name = "lms_role")]
		public LmsRoleType? LmsRoleType { get; set; }
	}
}