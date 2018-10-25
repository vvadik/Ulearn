using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses.Comments
{
	[DataContract]
	public class CreateCommentResponse
	{
		[DataMember(Name = "id")]
		public int CommentId { get; set; }
		
		[DataMember(Name = "api_url")]
		public string ApiUrl { get; set; }
	}
}