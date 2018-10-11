using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses
{
	[DataContract]
	public class SuccessResponse : ApiResponse
	{
		[DataMember(Name = "status")]
		public string Status { get; set; } = "ok";
		
		[DataMember(Name = "message", EmitDefaultValue = false)]
		public string Message { get; set; }

		public SuccessResponse(string message)
		{
			Message = message;
		}
	}
}