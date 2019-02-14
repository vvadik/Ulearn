using System.Collections.Generic;
using System.Runtime.Serialization;
using Database.Repos.Users;
using Ulearn.Common.Api.Models.Responses;
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

		/// <summary>
		/// Поле, по содержимому которого полнотекстовым поиском найден пользователь
		/// </summary>
		[DataMember(Name = "fields")]
		public List<SearchField> Fields { get; set; }
	}
}