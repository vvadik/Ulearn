using CommandLine;
using uLearn.CourseTool.Monitoring;

namespace uLearn.CourseTool.CmdLineOptions
{
	[Verb("monitor", HelpText = "start small http server to monitor all changes in ulearn course")]
	class MonitorOptions : AbstractOptions
	{
		public override void DoExecute()
		{
			Monitor.Start(Dir, Config.ULearnCourseId);
		}
	}
}