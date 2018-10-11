using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses
{
	[DataContract]
	public class ErrorResponse : ApiResponse
	{
		[DataMember(Name = "status")]
		public string Status { get; set; } = "error";
		
		[DataMember(Name = "message")]
		public string Message { get; set; }

		public ErrorResponse(string message)
		{
			Message = message;
		}
	}
}