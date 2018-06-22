using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Results.Account
{
	[DataContract]
	public class GetMeResponse
	{
		[DataMember(Name = "user")]
		public ShortUserInfo User { get; set; }
	}
}