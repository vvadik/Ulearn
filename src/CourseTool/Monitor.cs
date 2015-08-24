using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace uLearn.CourseTool
{
	class Monitor
	{
		private const int timeout = 10;
		static string courseDir = "BasicProgramming";
		static string htmlDir = "html";
		private static HttpServer server;

		public static void StartMonitor(string homeDir, string courseDir)
		{
			Monitor.courseDir = homeDir + "/" + courseDir;
			htmlDir = homeDir + "/" + htmlDir;
			Reload();
			var fileWatcher = new FileSystemWatcher(Monitor.courseDir);
			fileWatcher.IncludeSubdirectories = true;
			fileWatcher.Changed += FileWatcherOnChanged;
			fileWatcher.EnableRaisingEvents = true;
			Console.WriteLine("Started monitoring {0}", Monitor.courseDir);
			server = new HttpServer(htmlDir, 1337);
			server.Start();
			Process.Start(@"chrome", "http://localhost:1337/001.html -a");
			while (Console.ReadKey().Key != ConsoleKey.Q);
		}

		static void Reload(string name = null)
		{
			var course = new CourseLoader().LoadCourse(new DirectoryInfo(courseDir));
			Utils.DirectoryCopy(Utils.GetRootDirectory() + "/renderer", htmlDir, true);
			
			foreach (var slide in course.Slides.Where(x => name == null || x.Info.SlideFile.Name == name))
			{
				if (name != null)
				{
					Console.WriteLine("Reloading {0}", name);
					server.AddUpdate(slide.Index.ToString("000") + ".html");
				}
				File.WriteAllText(string.Format("{0}/{1}.html", htmlDir, slide.Index.ToString("000")), new SlideRenderer(new DirectoryInfo(htmlDir)).RenderSlide(course, slide));
			}
		}

		private static void FileWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			Console.WriteLine(fileSystemEventArgs.FullPath + " was changed, reloading all");
			while (true)
			try
			{
				Reload(new FileInfo(fileSystemEventArgs.FullPath).Name);
				break;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.InnerException);
			}
		}
	}
}
