using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CourseToolHotReloader.Dtos
{
	public class AccountTokenResponseDto
	{
		[JsonPropertyName("token")]
		public string Token { get; set; }

		[JsonPropertyName("status")]
		public string Status { get; set; }
	}

	public class TempCourseUpdateResponse
	{
		[JsonPropertyName("message")]
		public string Message { get; set; }

		[JsonPropertyName("errorType")]
		public ErrorType ErrorType { get; set; }

		[JsonPropertyName("lastUploadTime")]
		public DateTime LastUploadTime { get; set; }
	}

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum ErrorType
	{
		NoErrors,
		Forbidden,
		Conflict,
		NotFound,
		CourseError
	}
}