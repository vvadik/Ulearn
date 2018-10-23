using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses.Groups
{
	[DataContract]
	public class CreateGroupResponse : ApiResponse
	{
		[DataMember(Name = "id")]
		public int GroupId { get; set; }
		
		[DataMember(Name = "api_url")]
		public string ApiUrl { get; set; }
	}
}