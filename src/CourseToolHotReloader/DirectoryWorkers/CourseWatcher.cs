using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CourseToolHotReloader.UpdateQuery;

namespace CourseToolHotReloader.DirectoryWorkers
{
	public interface ICourseWatcher
	{
		public void StartWatch(string pathToCourse);
	}

	public class CourseWatcher : ICourseWatcher
	{
		private readonly ICourseUpdateSender courseUpdateSender;
		private readonly ICourseUpdateQuery courseUpdateQuery;
		private string path;
		private readonly Action debouncedSendUpdates;

		public CourseWatcher(ICourseUpdateQuery courseUpdateQuery, ICourseUpdateSender courseUpdateSender)
		{
			this.courseUpdateSender = courseUpdateSender;
			this.courseUpdateQuery = courseUpdateQuery;
			debouncedSendUpdates = Debounce(courseUpdateSender.SendCourseUpdates);
		}

		public void StartWatch(string pathToCourse)
		{
			// todo use pathToCourse;
			path = "C:\\Users\\holkin\\RiderProjects\\ConsoleHotReloader\\ConsoleHotReloader\\bin\\Debug\\netcoreapp3.0\\testFolder";
			WatchDirectory(Path.GetDirectoryName(path), RegisterUpdate);
		}

		private void RegisterUpdate(object _, FileSystemEventArgs fileSystemEventArgs)
		{
			var relativePath = fileSystemEventArgs.FullPath.Replace(path, "");

			var courseUpdate = CourseUpdateBuilder.Build(fileSystemEventArgs.Name, fileSystemEventArgs.FullPath, relativePath);

			courseUpdateQuery.Push(courseUpdate);

			debouncedSendUpdates();
		}


		private static void WatchDirectory(string directory, FileSystemEventHandler handler)
		{
			Console.WriteLine(directory);
			using var watcher = new FileSystemWatcher
			{
				Path = directory,
				NotifyFilter = NotifyFilters.LastAccess
								| NotifyFilters.LastWrite
								| NotifyFilters.FileName
								| NotifyFilters.DirectoryName,
				Filter = "*",
				IncludeSubdirectories = true
			};

			watcher.Changed += handler;
			watcher.Created += handler;
			watcher.Deleted += handler;

			watcher.EnableRaisingEvents = true;

			Console.WriteLine("Press 'q' to quit");
			while (Console.Read() != 'q') ;
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