using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CourseToolHotReloader
{
	public class HttpMethods
	{
		public static void TestGetJWTToken()
		{
			var task = GetJwtToken(new LoginPasswordParameters { Login = "admin", Password = "fullcontrol" });
			task.Wait();
			var accountTokenResponseDto = task.Result;
			var token = accountTokenResponseDto.Token;
			Console.WriteLine(token);
		}

		public static void TestCreateCourse()
		{
			const string path = "C:/Projects/ulearn/Ulearn/courses/Help/Help.zip";

			var task = CreateCourse(path);
			task.Wait();
		}

		private static async Task<AccountTokenResponseDto> GetJwtToken(LoginPasswordParameters parameters)
		{
			const string baseUrl = "http://localhost:8000";
			var url = $"{baseUrl}/account/login";

			var json = JsonSerializer.Serialize(parameters);
			var data = new StringContent(json, Encoding.UTF8, "application/json");

			using var client = new HttpClient();

			var response = await client.PostAsync(url, data);

			var result = response.Content.ReadAsStringAsync().Result;
			return JsonSerializer.Deserialize<AccountTokenResponseDto>(result);
		}

		private static async Task CreateCourse(string path)
		{
			const string baseUrl = "http://localhost:8000";
			var url = $"{baseUrl}/tempCourses/create/123";

			using var client = new HttpClient();

			var multiForm = new MultipartFormDataContent();
			var fileStream = File.OpenRead(path);
			var streamContent = new StreamContent(fileStream);
			multiForm.Add(streamContent, "files", Path.GetFileName(path));
			var response = await client.PostAsync(url, multiForm);

			var result = response.Content.ReadAsStringAsync().Result;
		}

		public class AccountTokenResponseDto
		{
			[JsonPropertyName("token")]
			public string Token { get; set; }

			[JsonPropertyName("status")]

			public string Status { get; set; }
		}

		private class LoginPasswordParameters
		{
			[JsonPropertyName("login")]
			public string Login { get; set; }

			[JsonPropertyName("password")]
			public string Password { get; set; }
		}
	}
}