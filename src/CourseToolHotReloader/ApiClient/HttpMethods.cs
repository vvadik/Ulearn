using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CourseToolHotReloader.Dtos;
using CourseToolHotReloader.Exceptions;
using CourseToolHotReloader.Log;
using JetBrains.Annotations;

namespace CourseToolHotReloader.ApiClient
{
	public interface IHttpMethods
	{
		Task<TokenResponseDto> GetJwtToken(LoginPasswordParameters parameters);
		Task<TempCourseUpdateResponse> UploadCourse(MemoryStream memoryStream, string id);
		Task<TempCourseUpdateResponse> UploadFullCourse(MemoryStream memoryStream, string id);
		Task<TempCourseUpdateResponse> CreateCourse(string id);
		Task<HasTempCourseResponse> HasCourse(string id);
		Task<TokenResponseDto> RenewToken();
	}

	public class HttpMethods : IHttpMethods
	{
		private readonly IConfig config;

		public HttpMethods(IConfig config)
		{
			this.config = config;
		}

		[ItemCanBeNull]
		public async Task<TokenResponseDto> GetJwtToken(LoginPasswordParameters parameters)
		{
			var url = $"{config.BaseUrl}/account/login";

			var json = JsonSerializer.Serialize(parameters);
			var data = new StringContent(json, Encoding.UTF8, "application/json");

			using var client = new HttpClient();

			var response = await client.PostAsync(url, data);

			if (response.StatusCode != HttpStatusCode.OK)
				return null;

			var result = response.Content.ReadAsStringAsync().Result;
			return JsonSerializer.Deserialize<TokenResponseDto>(result);
		}

		[ItemCanBeNull]
		public async Task<TokenResponseDto> RenewToken()
		{
			var url = $"{config.BaseUrl}/account/api-token?days=3";

			using var client = HttpClient();
			var response = await client.PostAsync(url, null);

			if (response.StatusCode != HttpStatusCode.OK)
				return null;

			var result = response.Content.ReadAsStringAsync().Result;
			return JsonSerializer.Deserialize<TokenResponseDto>(result);
		}

		public async Task<TempCourseUpdateResponse> UploadCourse(MemoryStream memoryStream, string id)
		{
			var url = $"{config.BaseUrl}/tempCourses/{id}";

			return await UpdateTempCourse(memoryStream, url, HttpMethod.Patch);
		}

		public async Task<TempCourseUpdateResponse> UploadFullCourse(MemoryStream memoryStream, string id)
		{
			var url = $"{config.BaseUrl}/tempCourses/{id}";

			return await UpdateTempCourse(memoryStream, url, HttpMethod.Put);
		}

		public async Task<TempCourseUpdateResponse> CreateCourse(string id)
		{
			var url = $"{config.BaseUrl}/tempCourses/{id}";

			using var client = HttpClient();

			var response = await client.PostAsync(url, null);

			BadCodeHandler(response);

			return DeserializeResponseContent<TempCourseUpdateResponse>(response);
		}


		public async Task<HasTempCourseResponse> HasCourse(string id)
		{
			var url = $"{config.BaseUrl}/tempCourses/byBaseCourseId/{id}";

			using var client = HttpClient();

			var response = await client.GetAsync(url);

			return response.StatusCode != HttpStatusCode.OK
				? null
				: DeserializeResponseContent<HasTempCourseResponse>(response);
		}

		private static T DeserializeResponseContent<T>(HttpResponseMessage response)
		{
			var result = response.Content.ReadAsStringAsync().Result;
			return JsonSerializer.Deserialize<T>(result);
		}

		private async Task<TempCourseUpdateResponse> UpdateTempCourse(MemoryStream memoryStream, string url, HttpMethod httpMethod)
		{
			using var client = HttpClient();

			var fileContent = new ByteArrayContent(memoryStream.ToArray());
			var multiContent = new MultipartFormDataContent { { fileContent, "files", "qwe.zip" } };

			var response = httpMethod == HttpMethod.Patch ? await client.PatchAsync(url, multiContent) : await client.PutAsync(url, multiContent); 

			BadCodeHandler(response);

			return DeserializeResponseContent<TempCourseUpdateResponse>(response);
		}

		private void BadCodeHandler(HttpResponseMessage response)
		{
			switch (response.StatusCode)
			{
				case HttpStatusCode.OK:
					return;
				case HttpStatusCode.Unauthorized:
					throw new UnauthorizedException("Срок авторизации истек, требуется повторная авторизация");
				case HttpStatusCode.Forbidden:
					throw new ForbiddenException("Нет прав для действий");
				case HttpStatusCode.InternalServerError:
					throw new InternalServerErrorException($"На сервере произошла ошибка: {DeserializeResponseContent<ServerErrorDto>(response).Message}");
			}
		}

		private HttpClient HttpClient()
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", config.JwtToken);
			return client;
		}
	}
}