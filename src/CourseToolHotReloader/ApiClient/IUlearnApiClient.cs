using System;
using System.Collections.Generic;
using CourseToolHotReloader.DirectoryWorkers;
using CourseToolHotReloader.Dtos;

namespace CourseToolHotReloader.ApiClient
{
    public interface IUlearnApiClient
    {
        void SendCourseUpdates(IList<ICourseUpdate> update);
    }

    class UlearnApiClient : IUlearnApiClient
    {
        private IUlearnApiClient _ulearnApiClientImplementation;

        public void SendCourseUpdates(IList<ICourseUpdate> updates)
		{
			var guid = ZipHelper.CreateNewZipByUpdates(updates);
			Console.WriteLine($"{guid}.zip created");
        }
    }
}