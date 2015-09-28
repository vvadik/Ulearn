using System.IO;
using CommandLine;
using System.Diagnostics;
using System;
using System.Reflection;

namespace uLearn.CourseTool
{
	[Verb("monitor", HelpText = "start small http server to monitor all changes in ulearn course")]
	class MonitorOptions : AbstractOptions
	{
		public override void DoExecute()
		{
            var process = new Process();
            var assembly = Assembly.GetExecutingAssembly().Location;
            var assemblyFile = new FileInfo(assembly);
            var assemblyFolder = assemblyFile.Directory;
            var filePath = Path.Combine(assemblyFolder.FullName, "ulearn.CourseMonitor.exe");
            var file = new FileInfo(filePath);
            if (!file.Exists)
            {
                Console.WriteLine("Missing " + file.Name);
                return;
            }
            process.StartInfo.FileName = file.FullName;
            process.StartInfo.Arguments = string.Format("\"{0}\" \"{1}\"", Dir, Config.ULearnCourseId);
           // process.StartInfo.UseShellExecute = true;
           // process.StartInfo.Verb = "runas";
            process.Start();
			//Monitor.Start(Dir, Config.ULearnCourseId);
		}
	}
}
