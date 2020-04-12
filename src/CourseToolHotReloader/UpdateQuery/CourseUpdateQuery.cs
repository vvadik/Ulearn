using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CourseToolHotReloader.Dtos;

namespace CourseToolHotReloader.UpdateQuery
{
	public interface ICourseUpdateQuery
	{
		void Push(ICourseUpdate update);
		IList<ICourseUpdate> GetAllCourseUpdate();
	}

	public class CourseUpdateQuery : ICourseUpdateQuery
	{
		private ConcurrentQueue<ICourseUpdate> query;
		private ConcurrentDictionary<string, ICourseUpdate> query1;

		public CourseUpdateQuery()
		{
			query = new ConcurrentQueue<ICourseUpdate>();
			query1 = new ConcurrentDictionary<string, ICourseUpdate>();
		}

		public void Push(ICourseUpdate update)
		{
			if (query1.ContainsKey(update.FullPath)) // todo remove
			{
				Console.WriteLine($"{update.Name} update to query");
			}
			else
			{
				Console.WriteLine($"{update.Name} add to query");
			}

			query1.AddOrUpdate(update.FullPath, update, (_1, _2) => update);
		}

		public IList<ICourseUpdate> GetAllCourseUpdate()
		{
			var result = query1.Values.ToArray();
			query1.Clear();
			return result;
		}
	}
}