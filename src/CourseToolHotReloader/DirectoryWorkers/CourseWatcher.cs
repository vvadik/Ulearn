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
		public void StartWatch(string pathToCourse);
	}

	public class CourseWatcher : ICourseWatcher
	{
		private readonly ICourseUpdateQuery courseUpdateQuery;
		private string path;
		private readonly Action debouncedSendUpdates;

		public CourseWatcher(ICourseUpdateQuery courseUpdateQuery, ICourseUpdateSender courseUpdateSender)
		{
			this.courseUpdateQuery = courseUpdateQuery;
			debouncedSendUpdates = Debounce(courseUpdateSender.SendCourseUpdates);
		}

		public void StartWatch(string pathToCourse)
		{
			// todo use pathToCourse;
			path = "C:\\Users\\holkin\\RiderProjects\\ConsoleHotReloader\\ConsoleHotReloader\\bin\\Debug\\netcoreapp3.0\\testFolder";
			WatchDirectory(path);
		}

		private ICourseUpdate BuildCourseUpdateBuFileSystemEvent(FileSystemEventArgs fileSystemEventArgs)
		{
			var relativePath = fileSystemEventArgs.FullPath.Replace(path, "");

			var courseUpdate = CourseUpdateBuilder.Build(fileSystemEventArgs.Name, fileSystemEventArgs.FullPath, relativePath);
			return courseUpdate;
		}

		private void WatchDirectory(string directory)
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

			watcher.Changed += Changed;
			watcher.Created += Created;
			watcher.Deleted += Deleted;
			watcher.Renamed += Renamed;

			watcher.EnableRaisingEvents = true;

			Console.WriteLine("Press 'q' to quit");
			while (Console.Read() != 'q') // todo не могу вынести
			{
			}
		}

		private void Renamed(object sender, RenamedEventArgs e)
		{
			var relativePath = e.OldFullPath.Replace(path, "");
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

		private void Changed(object sender, FileSystemEventArgs e)
		{
			if (Directory.Exists(e.FullPath))
				return; //todo немного странно
			var courseUpdate = BuildCourseUpdateBuFileSystemEvent(e);
			courseUpdateQuery.RegisterUpdate(courseUpdate);
			debouncedSendUpdates();
		}


		private static Action Debounce(Action func, int milliseconds = 10000)
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