using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace uLearn.CourseTool
{
	class Monitor
	{
		static string courseDir = "BasicProgramming";
		static string htmlDir = "html";
		private static HttpServer server;

		public static void StartMonitor(string homeDir, string courseDir)
		{
			Monitor.courseDir = homeDir + "/" + courseDir;
			htmlDir = homeDir + "/" + htmlDir;
			if (!Directory.Exists(htmlDir))
				Directory.CreateDirectory(htmlDir);
			if (!Directory.Exists(htmlDir + "/static"))
				Directory.CreateDirectory(htmlDir + "/static");
			Utils.DirectoryCopy(Utils.GetRootDirectory() + "/renderer", htmlDir, true);
			server = new HttpServer(htmlDir, 1337);
			Reload();
			var fileWatcher = new FileSystemWatcher(Monitor.courseDir);
			fileWatcher.IncludeSubdirectories = true;
			fileWatcher.Changed += FileWatcherOnChanged;
			fileWatcher.EnableRaisingEvents = true;
			Console.WriteLine("Started monitoring {0}", Monitor.courseDir);
			server.Start();
			Process.Start(@"chrome", "http://localhost:1337/001.html -a");
			while (Console.ReadKey().Key != ConsoleKey.Q);
		}

		static void Reload()
		{
			var course = new CourseLoader().LoadCourse(new DirectoryInfo(courseDir));
			server.course = course;
			var renderer = new SlideRenderer(new DirectoryInfo(htmlDir));
			foreach (var slide in course.Slides)
				File.WriteAllText(
					string.Format("{0}/{1}.html", htmlDir, slide.Index.ToString("000")), 
					renderer.RenderSlide(course, slide)
				);
			foreach (var note in course.GetUnits().Select(course.FindInstructorNote).Where(x => x != null))
				File.WriteAllText(
					string.Format("{0}/{1}.html", htmlDir, note.UnitName),
					renderer.RenderInstructorsNote(course, note.UnitName)
				);
			server.UpdateAll();
		}

		private static void FileWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			Console.WriteLine("{0} was {1}.", fileSystemEventArgs.FullPath, fileSystemEventArgs.ChangeType.ToString().ToLower());
			while (true)
				try
				{
					Reload();
					break;
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
		}
	}
}
