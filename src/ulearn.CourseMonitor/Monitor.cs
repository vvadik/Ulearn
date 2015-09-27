using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;

namespace uLearn.CourseTool
{
	class Monitor
	{
		private readonly PreviewHttpServer server;
		private readonly string courseDir;

    	public static void Start(string homeDir, string courseId)
		{
			// ReSharper disable once ObjectCreationAsStatement
			new Monitor(homeDir, courseId);
		}

		private Monitor(string homeDir, string courseId)
		{
			courseDir = Path.Combine(homeDir, courseId);
			server = new PreviewHttpServer(courseDir, Path.Combine(homeDir, "html"), 1337);
			server.Start();
			StartWatchingCourseDir();
			Console.WriteLine("Started monitoring {0}", courseDir);
			OpenInBrowser();
			while (true)
			{
				var key = Console.ReadKey(intercept:true).Key;
				if (key == ConsoleKey.Q)
					break;
				else if (key == ConsoleKey.O)
					OpenInBrowser();
				else
					Console.WriteLine("Press 'Q' to exit. Press 'O' to open course in browser");
			}
		}

		private void StartWatchingCourseDir()
		{
			var fileWatcher = new FileSystemWatcher(courseDir) { IncludeSubdirectories = true };
			fileWatcher.Changed += FileWatcherOnChanged;
			fileWatcher.Renamed += FileWatcherOnChanged;
			fileWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName;
			fileWatcher.EnableRaisingEvents = true;
		}

		private static void OpenInBrowser()
		{
			Process.Start(@"http://localhost:1337/001.html");
		}

		private void FileWatcherOnChanged(object sender, FileSystemEventArgs args)
		{
			Console.WriteLine("{0} {1} was {2}.", DateTime.Now.ToString("T"), args.Name, args.ChangeType.ToString().ToLower());
			server.MarkCourseAsChanged();
		}
	}
}
