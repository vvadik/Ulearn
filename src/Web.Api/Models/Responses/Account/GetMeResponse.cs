using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Account
{
	[DataContract]
	public class GetMeResponse : ApiResponse
	{
		[DataMember(Name = "is_authenticated")]
		public bool IsAuthenticated { get; set; }
		
		[DataMember(Name = "user", EmitDefaultValue = false)]
		public ShortUserInfo User { get; set; }
	}
}