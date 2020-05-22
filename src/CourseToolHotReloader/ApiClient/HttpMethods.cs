using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CourseToolHotReloader.Dtos;

namespace CourseToolHotReloader.ApiClient
{
	public class HttpMethods
	{
		public static async Task<AccountTokenResponseDto> GetJwtToken(LoginPasswordParameters parameters)
		{
			const string baseUrl = "http://localhost:8000";
			var url = $"{baseUrl}/account/login";

			var json = JsonSerializer.Serialize(parameters);
			var data = new StringContent(json, Encoding.UTF8, "application/json");

			using var client = new HttpClient();

			var response = await client.PostAsync(url, data);

			if (response.StatusCode != HttpStatusCode.OK)
				throw new Exception("Неправильный логин пароль");

			var result = response.Content.ReadAsStringAsync().Result;
			return JsonSerializer.Deserialize<AccountTokenResponseDto>(result);
		}

		private static async Task UploadCourse(string path, string token)
		{
			const string baseUrl = "http://localhost:8000";
			var url = $"{baseUrl}/tempCourses/uploadCourse/123";

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", token);

			var multiForm = new MultipartFormDataContent();
			var fileStream = File.OpenRead(path);
			var streamContent = new StreamContent(fileStream);
			multiForm.Add(streamContent, "files", Path.GetFileName(path));
			var response = await client.PostAsync(url, multiForm);

			var result = response.Content.ReadAsStringAsync().Result;
		}


		public static async Task UploadCourse(MemoryStream memoryStream, string token, string id)
		{
			const string baseUrl = "http://localhost:8000";
			var url = $"{baseUrl}/tempCourses/uploadCourse/{id}";

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", token);

			var fileContent = new ByteArrayContent(memoryStream.ToArray());
			var multiContent = new MultipartFormDataContent { { fileContent, "files", "qwe.zip" } };
			var response = await client.PostAsync(url, multiContent);

			if (response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine($"we have error {response.Content.ReadAsStringAsync().Result}");
			}
		}

		public static async Task CreateCourse(string token, string id)
		{
			const string baseUrl = "http://localhost:8000";
			var url = $"{baseUrl}/tempCourses/create/{id}";

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", token);

			var response = await client.PostAsync(url, null);

			if (response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine($"We have error: {response.Content.ReadAsStringAsync().Result}");
			}
		}
	}
}