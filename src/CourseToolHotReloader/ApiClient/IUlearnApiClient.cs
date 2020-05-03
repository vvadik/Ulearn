using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CourseToolHotReloader.DirectoryWorkers;
using CourseToolHotReloader.Dtos;

namespace CourseToolHotReloader.ApiClient
{
    public interface IUlearnApiClient
    {
        void SendCourseUpdates(IList<ICourseUpdate> update, IList<ICourseUpdate> deletedFiles);
		Task SendFullCourse(string path, string token);

	}

    class UlearnApiClient : IUlearnApiClient
    {
        private IUlearnApiClient _ulearnApiClientImplementation;

        public void SendCourseUpdates(IList<ICourseUpdate> updates, IList<ICourseUpdate> deletedFiles)
		{
			var guid = ZipUpdater.CreateZipByUpdates(updates, deletedFiles);
			Console.WriteLine($"{guid}.zip created");
        }
		
        public async Task SendFullCourse(string path, string token)
		{
			var ms = ZipUpdater.CreateZipByFolder(path);

			await HttpMethods.UploadCourse(ms, token);
			
			
			Console.WriteLine($"{ms}.zip created");
        }
    }
}