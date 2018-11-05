using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Users
{
	[DataContract]
	public class InstructorsListResponse : SuccessResponse
	{
		[DataMember(Name = "instructors")]
		public List<ShortUserInfo> Instructors { get; set; }
	}
}