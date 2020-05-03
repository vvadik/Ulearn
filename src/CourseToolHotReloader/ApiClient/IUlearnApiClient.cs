using System;
using System.Collections.Generic;
using CourseToolHotReloader.DirectoryWorkers;
using CourseToolHotReloader.Dtos;

namespace CourseToolHotReloader.ApiClient
{
    public interface IUlearnApiClient
    {
        void SendCourseUpdates(IList<ICourseUpdate> update, IList<ICourseUpdate> deletedFiles);
    }

    class UlearnApiClient : IUlearnApiClient
    {
        private IUlearnApiClient _ulearnApiClientImplementation;

        public void SendCourseUpdates(IList<ICourseUpdate> updates, IList<ICourseUpdate> deletedFiles)
		{
			var guid = ZipUpdater.CreateZipByUpdates(updates, deletedFiles);
			Console.WriteLine($"{guid}.zip created");
        }
    }
}