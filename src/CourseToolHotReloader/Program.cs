using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CourseToolHotReloader
{
	internal class Program
	{
		private static void Main()
		{
			var task = GetJwtTokenModno(new LoginPasswordParameters { Login = "***", Password = "***" });
			task.Wait();
			var accountTokenResponseDto = task.Result;
			var token = accountTokenResponseDto.Token;
			Console.WriteLine(token);
		}

		private static async Task<AccountTokenResponseDto> GetJwtToken(LoginPasswordParameters parameters)
		{
			const string baseUrl = "http://localhost:8000";
			var url = $"{baseUrl}/account/login";
			//todo навернео в проекте есть способ делать запросы, но я пока так напишу, нужно сделать как и везде
			var request = WebRequest.Create(url);
			// а как сделать деконструкцию здесь???
			request.Method = "POST";
			request.ContentType = "application/json";

			var data = JsonSerializer.Serialize(parameters);
			var byteArray = Encoding.UTF8.GetBytes(data);
			request.ContentLength = byteArray.Length;

			await using var dataStream = request.GetRequestStream();
			dataStream.Write(byteArray, 0, byteArray.Length);

			using var response = await request.GetResponseAsync();
			await using var stream = response.GetResponseStream();
			return await JsonSerializer.DeserializeAsync<AccountTokenResponseDto>(stream);
		}

		private static async Task<AccountTokenResponseDto> GetJwtTokenModno(LoginPasswordParameters parameters)
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