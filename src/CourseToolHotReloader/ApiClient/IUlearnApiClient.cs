using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CourseToolHotReloader.DirectoryWorkers;
using CourseToolHotReloader.Dtos;

namespace CourseToolHotReloader.ApiClient
{
	public interface IUlearnApiClient
	{
		Task SendCourseUpdates(IList<ICourseUpdate> update, IList<ICourseUpdate> deletedFiles, string token, string courseId);
		Task SendFullCourse(string path, string token, string courseId);
	}

	class UlearnApiClient : IUlearnApiClient
	{
		public async Task SendCourseUpdates(IList<ICourseUpdate> updates, IList<ICourseUpdate> deletedFiles, string token, string courseId)
		{
			var ms = ZipUpdater.CreateZipByUpdates(updates, deletedFiles);

			var updateResponse = await HttpMethods.UploadCourse(ms, token, courseId);
			
			Console.WriteLine($"{courseId} updates only upload");
		}

		public async Task SendFullCourse(string path, string token, string courseId)
		{
			var ms = ZipUpdater.CreateZipByFolder(path);

			var updateResponse = await HttpMethods.UploadFullCourse(ms, token, courseId);

			Console.WriteLine($"{courseId} upload");
		}
	}
}