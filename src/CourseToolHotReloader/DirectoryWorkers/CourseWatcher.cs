using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CourseToolHotReloader.Dtos;
using CourseToolHotReloader.UpdateQuery;

namespace CourseToolHotReloader.DirectoryWorkers
{
	public interface ICourseWatcher
	{
		public void StartWatch(bool sendFullDirectoryOnChange);
	}

	public class CourseWatcher : ICourseWatcher
	{
		private readonly ICourseUpdateQuery courseUpdateQuery;
		private readonly IConfig config;
		private readonly Action debouncedSendUpdates;
		private readonly Action debouncedSendFullCourse;

		public CourseWatcher(ICourseUpdateQuery courseUpdateQuery, ICourseUpdateSender courseUpdateSender, IConfig config)
		{
			this.courseUpdateQuery = courseUpdateQuery;
			this.config = config;
			debouncedSendUpdates = Debounce(courseUpdateSender.SendCourseUpdates);
			debouncedSendFullCourse = Debounce(courseUpdateSender.SendFullCourse);
		}

		public void StartWatch(bool sendFullDirectoryOnChange)
		{
			if (sendFullDirectoryOnChange)
			{
				WatchDirectory(config.Path, ChangedForSendAll, ChangedForSendAll, ChangedForSendAll, ChangedForSendAll);
			}
			else
			{
				WatchDirectory(config.Path, Changed, Created, Deleted, Renamed);
			}
		}

		private ICourseUpdate BuildCourseUpdateBuFileSystemEvent(FileSystemEventArgs fileSystemEventArgs)
		{
			var relativePath = fileSystemEventArgs.FullPath.Replace(config.Path, "");

			var courseUpdate = CourseUpdateBuilder.Build(fileSystemEventArgs.Name, fileSystemEventArgs.FullPath, relativePath);
			return courseUpdate;
		}

		private static void WatchDirectory(string directory,
			FileSystemEventHandler changed, FileSystemEventHandler created, FileSystemEventHandler deleted, RenamedEventHandler renamed)
		{
			using var watcher = new FileSystemWatcher
			{
				Path = directory,
				NotifyFilter = NotifyFilters.LastWrite
								| NotifyFilters.DirectoryName
								| NotifyFilters.FileName,
				Filter = "*",
				IncludeSubdirectories = true
			};

			watcher.Changed += changed;
			watcher.Created += created;
			watcher.Deleted += deleted;
			watcher.Renamed += renamed;

			watcher.EnableRaisingEvents = true;

			Console.WriteLine("Press 'q' to quit");
			while (Console.Read() != 'q') // todo не могу вынести
			{
			}
		}

		private void Renamed(object sender, RenamedEventArgs e)
		{
			var relativePath = e.OldFullPath.Replace(config.Path, "");
			var deletedCourseUpdate = CourseUpdateBuilder.Build(e.OldName, e.OldFullPath, relativePath);
			courseUpdateQuery.RegisterDelete(deletedCourseUpdate);

			var courseUpdate = BuildCourseUpdateBuFileSystemEvent(e);
			courseUpdateQuery.RegisterCreate(courseUpdate);

			debouncedSendUpdates();
		}

		private void Deleted(object sender, FileSystemEventArgs e)
		{
			var courseUpdate = BuildCourseUpdateBuFileSystemEvent(e);
			courseUpdateQuery.RegisterDelete(courseUpdate);
			debouncedSendUpdates();
		}

		private void Created(object sender, FileSystemEventArgs e)
		{
			var courseUpdate = BuildCourseUpdateBuFileSystemEvent(e);
			courseUpdateQuery.RegisterCreate(courseUpdate);
			debouncedSendUpdates();
		}

		private void ChangedForSendAll(object sender, FileSystemEventArgs e)
		{
			debouncedSendFullCourse();
		}

		private void Changed(object sender, FileSystemEventArgs e)
		{
			if (Directory.Exists(e.FullPath))
				return; //todo немного странно
			var courseUpdate = BuildCourseUpdateBuFileSystemEvent(e);
			courseUpdateQuery.RegisterUpdate(courseUpdate);
			debouncedSendUpdates();
		}


		private static Action Debounce(Action func, int milliseconds = 5000)
		{
			CancellationTokenSource cancelTokenSource = null;
			return () =>
			{
				cancelTokenSource?.Cancel();
				cancelTokenSource = new CancellationTokenSource();

				Task.Delay(milliseconds, cancelTokenSource.Token)
					.ContinueWith(t =>
					{
						if (t.IsCompletedSuccessfully)
							func();
					}, TaskScheduler.Default);
			};
		}
	}
}