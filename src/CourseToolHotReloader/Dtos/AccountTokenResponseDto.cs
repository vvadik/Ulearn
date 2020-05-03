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
}