using System.Runtime.Serialization;
using Ulearn.Common.Api.Models.Responses;

namespace Ulearn.Web.Api.Models.Responses.Groups
{
	[DataContract]
	public class CreateGroupResponse : SuccessResponse
	{
		[DataMember(Name = "id")]
		public int GroupId { get; set; }
		
		[DataMember(Name = "api_url")]
		public string ApiUrl { get; set; }
	}
}