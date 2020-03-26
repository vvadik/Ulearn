using System;
using System.Collections.Generic;

namespace CourseToolHotReloader
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
            foreach (var courseUpdate in updates)
            {
                Console.WriteLine(courseUpdate.Name);
            }
        }
    }
}