using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses.Account
{
	[DataContract]
	public class TokenResponse : ApiResponse
	{
		[DataMember(Name = "token")]
		public string Token { get; set; }
	}
}