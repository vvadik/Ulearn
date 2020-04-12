using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CourseToolHotReloader.Dtos;

namespace CourseToolHotReloader.UpdateQuery
{
	public interface ICourseUpdateQuery
	{
		void RegisterUpdate(ICourseUpdate update);
		void RegisterDelete(ICourseUpdate update);
		IList<ICourseUpdate> GetAllCourseUpdate();
		IList<ICourseUpdate> GetAllDeletedFiles();
	}

	public class CourseUpdateQuery : ICourseUpdateQuery
	{
		private ConcurrentDictionary<string, ICourseUpdate> updatesQuery;
		private ConcurrentBag<ICourseUpdate> deletedFiles;

		public CourseUpdateQuery()
		{
			updatesQuery = new ConcurrentDictionary<string, ICourseUpdate>();
			deletedFiles = new ConcurrentBag<ICourseUpdate>();
		}

		public void RegisterUpdate(ICourseUpdate update)
		{
			if (updatesQuery.ContainsKey(update.FullPath)) // todo remove
			{
				Console.WriteLine($"{update.Name} update to query");
			}
			else
			{
				Console.WriteLine($"{update.Name} add to query");
			}

			updatesQuery.AddOrUpdate(update.FullPath, update, (_1, _2) => update);
		}

		public void RegisterDelete(ICourseUpdate update)
		{
			if (deletedFiles.Contains(update)) // todo remove
			{
				Console.WriteLine($"{update.Name} error?");
			}
			else
			{
				Console.WriteLine($"{update.Name} add to delete query");
			}

			updatesQuery.TryRemove(update.FullPath, out _);
			deletedFiles.Add(update);
		}

		public IList<ICourseUpdate> GetAllCourseUpdate()
		{
			var result = updatesQuery.Values.ToArray();
			updatesQuery.Clear();
			return result;
		}

		public IList<ICourseUpdate> GetAllDeletedFiles()
		{
			var result = deletedFiles.ToArray();
			deletedFiles.Clear();
			return result;
		}
	}
}