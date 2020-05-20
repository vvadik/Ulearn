using System;
using System.IO;
using CourseToolHotReloader.Dtos;
using CourseToolHotReloader.UpdateQuery;

namespace CourseToolHotReloader.DirectoryWorkers
{
	
	public interface ISendOnlyChangedStrategy : IWatchActionStrategy
	{
	}
	
	public class SendOnlyChangedStrategy : ISendOnlyChangedStrategy
	{
		private readonly ICourseUpdateQuery courseUpdateQuery;
		private readonly IConfig config;
		private readonly Action debouncedSendUpdates;

		public SendOnlyChangedStrategy(ICourseUpdateQuery courseUpdateQuery, ICourseUpdateSender courseUpdateSender, IConfig config)
		{
			this.courseUpdateQuery = courseUpdateQuery;
			this.config = config;
			debouncedSendUpdates = ActionHelper.Debounce(courseUpdateSender.SendCourseUpdates);
		}

		public void Renamed(object sender, RenamedEventArgs e)
		{
			var relativePath = e.OldFullPath.Replace(config.Path, "");
			var deletedCourseUpdate = CourseUpdateBuilder.Build(e.OldName, e.OldFullPath, relativePath);
			courseUpdateQuery.RegisterDelete(deletedCourseUpdate);

			var courseUpdate = BuildCourseUpdateByFileSystemEvent(e);
			courseUpdateQuery.RegisterCreate(courseUpdate);

			debouncedSendUpdates();
		}

		public void Deleted(object sender, FileSystemEventArgs e)
		{
			var courseUpdate = BuildCourseUpdateByFileSystemEvent(e);
			courseUpdateQuery.RegisterDelete(courseUpdate);
			debouncedSendUpdates();
		}

		public void Created(object sender, FileSystemEventArgs e)
		{
			var courseUpdate = BuildCourseUpdateByFileSystemEvent(e);
			courseUpdateQuery.RegisterCreate(courseUpdate);
			debouncedSendUpdates();
		}

		public void Changed(object sender, FileSystemEventArgs e)
		{
			if (Directory.Exists(e.FullPath))
				return; //todo немного странно
			var courseUpdate = BuildCourseUpdateByFileSystemEvent(e);
			courseUpdateQuery.RegisterUpdate(courseUpdate);
			debouncedSendUpdates();
		}
		
		private ICourseUpdate BuildCourseUpdateByFileSystemEvent(FileSystemEventArgs fileSystemEventArgs)
		{
			var relativePath = fileSystemEventArgs.FullPath.Replace(config.Path, "");

			var courseUpdate = CourseUpdateBuilder.Build(fileSystemEventArgs.Name, fileSystemEventArgs.FullPath, relativePath);
			return courseUpdate;
		}
	}
}