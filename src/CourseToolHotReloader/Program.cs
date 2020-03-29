using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using CommandLine;

namespace CourseToolHotReloader
{
	internal class Program
	{
		private static IContainer container;

		private static void Main(string[] args)
		{
			//AutofacDoMagic();
			TestMain();
			//Parser.Default.ParseArguments<Options>(args).WithParsed(Process);
		}

		private static void AutofacDoMagic()
		{
			var containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterModule(new ControllerDependencyModule());
			container = containerBuilder.Build();
		}

		private static void Process(Options options)
		{
			WatchDirectory(Path.GetDirectoryName(options.Path), Debounce(RegisterUpdate));
		}

		private static void RegisterUpdate(object _, FileSystemEventArgs fileSystemEventArgs)
		{
			var courseUpdate = new CourseUpdate
			{
				Name = fileSystemEventArgs.Name,
				RelativePath = fileSystemEventArgs.FullPath
			};

			container.Resolve<ICourseUpdateQuery>().Push(courseUpdate);
			container.Resolve<ICourseUpdateSender>().SendCourseUpdates();
		}

		private static void WatchDirectory(string directory, FileSystemEventHandler handler)
		{
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


		private static FileSystemEventHandler Debounce(FileSystemEventHandler func, int milliseconds = 1000)
		{
			CancellationTokenSource cancelTokenSource = null;
			return (arg1, arg2) =>
			{
				cancelTokenSource?.Cancel();
				cancelTokenSource = new CancellationTokenSource();

				Task.Delay(milliseconds, cancelTokenSource.Token)
					.ContinueWith(t =>
					{
						if (t.IsCompletedSuccessfully)
							func(arg1, arg2);
					}, TaskScheduler.Default);
			};
		}

		private static void TestMain()
		{
			HttpMethods.TestCreateCourse();
		}
	}
}