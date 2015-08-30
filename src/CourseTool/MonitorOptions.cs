using System.IO;
using CommandLine;

namespace uLearn.CourseTool
{
	[Verb("monitor", HelpText = "start small http server to monitor all changes in ulearn course")]
	class MonitorOptions : AbstractOptions
	{
		public override void DoExecute()
		{
			Monitor.StartMonitor(Dir, Config.ULearnCourseId);
		}
	}
}
