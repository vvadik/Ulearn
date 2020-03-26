using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CourseToolHotReloader
{
    public interface ICourseUpdateQuery
    {
        void Push(ICourseUpdate update);
        IList<ICourseUpdate> GetAllCourseUpdate();
    }

    public class CourseUpdateQuery : ICourseUpdateQuery
    {
        private ConcurrentQueue<ICourseUpdate> query;

        public CourseUpdateQuery()
        {
            query = new ConcurrentQueue<ICourseUpdate>();
        }

        public void Push(ICourseUpdate update)
        {
            query.Enqueue(update);
        }

        public IList<ICourseUpdate> GetAllCourseUpdate()
        {
            var result = query.ToArray();
            query.Clear();           
            return result;
        }
    }

}