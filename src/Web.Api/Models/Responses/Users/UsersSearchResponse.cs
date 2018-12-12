using System.Collections.Generic;
using System.Runtime.Serialization;
using Database.Repos.Users;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Users
{
	[DataContract]
	public class UsersSearchResponse : PaginatedResponse
	{
		[DataMember(Name = "users")]
		public List<FoundUserResponse> Users { get; set; }
	}

	public class FoundUserResponse
	{
		[DataMember(Name = "user")]
		public ShortUserInfo User { get; set; }
		
		[DataMember(Name = "fields")]
		public List<SearchField> Fields { get; set; }
	}
}