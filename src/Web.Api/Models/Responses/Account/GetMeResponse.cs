using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Account
{
	[DataContract]
	public class GetMeResponse : SuccessResponse
	{
		[DataMember(Name = "is_authenticated")]
		public bool IsAuthenticated { get; set; }
		
		[DataMember(Name = "user", EmitDefaultValue = false)]
		public ShortUserInfo User { get; set; }

		[DataMember(Name = "account_problems", EmitDefaultValue = false)]
		public List<AccountProblem> AccountProblems { get; set; }
	}

	[DataContract]
	public class AccountProblem
	{
		[DataMember(Name = "title")]
		public string Title { get; set; }
		
		[DataMember(Name = "description")]
		public string Description { get; set; }

		public AccountProblem(string title, string description)
		{
			Title = title;
			Description = description;
		}
	}
}