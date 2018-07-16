using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Courses
{
	[DataContract]
	public class CoursesListResponse : ApiResponse
	{
		[DataMember(Name = "courses")]
		public List<ShortCourseInfo> Courses { get; set; }
	}
}