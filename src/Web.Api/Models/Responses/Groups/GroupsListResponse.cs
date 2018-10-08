using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses.Groups
{
	[DataContract]
	public class GroupsListResponse : PaginatedResponse
	{
		[DataMember(Name = "groups")]
		public List<GroupInfo> Groups { get; set; }
	}
}