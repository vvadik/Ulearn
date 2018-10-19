using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Ulearn.Web.Api.Models.Parameters.Groups
{
	public class CopyGroupParameters
	{
		[Required]
		[FromQuery(Name = "destination_course_id")]
		public string DestinationCourseId { get; set; }

		[FromQuery(Name = "make_me_owner")]
		public bool ChangeOwner { get; set; } = false;
	}
}