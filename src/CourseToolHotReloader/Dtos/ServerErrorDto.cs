using System.Text.Json.Serialization;

namespace CourseToolHotReloader.Dtos
{
	public class ServerErrorDto
	{
		[JsonPropertyName("status")]
		public string Status { get; set; }

		[JsonPropertyName("message")]
		public string Message { get; set; }
		
		[JsonPropertyName("trace_id")]
		public string TraceId { get; set; }

		[JsonPropertyName("timestamp")]
		public string Timestamp { get; set; }
	}
}