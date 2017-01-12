using System.IO;
using CommandLine;
using System.Diagnostics;
using System;
using System.Reflection;
using uLearn.CourseTool.Monitoring;

namespace uLearn.CourseTool
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
