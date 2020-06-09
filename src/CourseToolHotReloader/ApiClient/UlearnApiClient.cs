using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CourseToolHotReloader.DirectoryWorkers;
using CourseToolHotReloader.Dtos;
using CourseToolHotReloader.Log;
using JetBrains.Annotations;

namespace CourseToolHotReloader.ApiClient
{
	public interface IUlearnApiClient
	{
		Task<TempCourseUpdateResponse> SendCourseUpdates(IList<ICourseUpdate> update, IList<ICourseUpdate> deletedFiles, string courseId);
		Task<TempCourseUpdateResponse> SendFullCourse(string path, string courseId);
		Task<TempCourseUpdateResponse> CreateCourse(string courseId);
		Task<string> Login(string login, string password);
		Task<HasTempCourseResponse> HasCourse(string courseId);
		Task<string> RenewToken();
	}

	internal class UlearnApiClient : IUlearnApiClient
	{
		private readonly IHttpMethods httpMethods;

		public UlearnApiClient(IHttpMethods httpMethods)
		{
			this.httpMethods = httpMethods;
		}

		public async Task<TempCourseUpdateResponse> SendCourseUpdates(IList<ICourseUpdate> updates, IList<ICourseUpdate> deletedFiles, string courseId)
		{
			var ms = ZipUpdater.CreateZipByUpdates(updates, deletedFiles);

			return await httpMethods.UploadCourse(ms, courseId);
		}

		[ItemCanBeNull]
		public async Task<TempCourseUpdateResponse> SendFullCourse(string path, string courseId)
		{
			var ms = ZipUpdater.CreateZipByFolder(path);

			return await httpMethods.UploadFullCourse(ms, courseId);
		}
		
		public async Task<TempCourseUpdateResponse> CreateCourse(string courseId)
		{
			return await httpMethods.CreateCourse(courseId);
		}

		[ItemCanBeNull]
		public async Task<HasTempCourseResponse> HasCourse(string courseId)
		{
			return await httpMethods.HasCourse(courseId);
		}

		[ItemCanBeNull]
		public async Task<string> Login(string login, string password)
		{
			var loginPasswordParameters = new LoginPasswordParameters
			{
				Login = login,
				Password = password
			};

			var accountTokenResponseDto = await httpMethods.GetJwtToken(loginPasswordParameters);
			return accountTokenResponseDto?.Token;
		}
		
		[ItemCanBeNull]
		public async Task<string> RenewToken()
		{
			var accountTokenResponseDto = await httpMethods.RenewToken();
			return accountTokenResponseDto?.Token;
		}
	}
}