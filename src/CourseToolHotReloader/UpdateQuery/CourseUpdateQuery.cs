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
		void RegisterCreate(ICourseUpdate courseUpdate);
		IList<ICourseUpdate> GetAllCourseUpdate();
		IList<ICourseUpdate> GetAllDeletedFiles();
	}

	public class CourseUpdateQuery : ICourseUpdateQuery
	{
		private readonly ConcurrentDictionary<string, ICourseUpdate> updatesQuery;
		private readonly ConcurrentDictionary<string, ICourseUpdate> deletedFiles;
		private readonly ConcurrentDictionary<string, ICourseUpdate> createdFiles;

		public CourseUpdateQuery()
		{
			updatesQuery = new ConcurrentDictionary<string, ICourseUpdate>();
			deletedFiles = new ConcurrentDictionary<string, ICourseUpdate>();
			createdFiles = new ConcurrentDictionary<string, ICourseUpdate>();
		}

		public void RegisterUpdate(ICourseUpdate update)
		{
			updatesQuery.AddOrUpdate(update.FullPath, update, (_1, _2) => update);
		}

		public void RegisterCreate(ICourseUpdate update)
		{
			updatesQuery.AddOrUpdate(update.FullPath, update, (_1, _2) => update);

			deletedFiles.TryRemove(update.FullPath, out _);
			
			createdFiles.AddOrUpdate(update.FullPath, update, (_1, _2) => update);
		}

		public void RegisterDelete(ICourseUpdate update)
		{
			updatesQuery.TryRemove(update.FullPath, out _);

			if (!createdFiles.TryRemove(update.FullPath, out _))
			{
				deletedFiles.AddOrUpdate(update.FullPath, update, (_1, _2) => update);
			}
		}

		public IList<ICourseUpdate> GetAllCourseUpdate()
		{
			var result = updatesQuery.Values.ToArray();
			updatesQuery.Clear();
			return result;
		}

		public IList<ICourseUpdate> GetAllDeletedFiles()
		{
			var result = deletedFiles.Values.ToArray();
			deletedFiles.Clear();
			return result;
		}
	}
}