using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses.Account
{
	[DataContract]
	public class LogoutResponse : SuccessResponse
	{
		[DataMember(Name = "logout")]
		public bool Logout { get; set; }
	}
}