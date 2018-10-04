using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses.Groups
{
	[DataContract]
	public class GroupsListResponse : ApiResponse
	{
		[DataMember(Name = "groups")]
		public List<GroupInfo> Groups { get; set; }
	}
}