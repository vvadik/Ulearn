using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Results.Account
{
	[DataContract]
	public class GetMeResponse
	{
		[DataMember(Name = "is_authenticated")]
		public bool IsAuthenticated { get; set; }
		
		[DataMember(Name = "user", EmitDefaultValue = false)]
		public ShortUserInfo User { get; set; }
	}
}