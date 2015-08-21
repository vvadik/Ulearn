using System.IO;
using CommandLine;

namespace uLearn.CourseTool
{
	[Verb("monitor", HelpText = "")]
	class MonitorOptions : AbstractOptions
	{
		public override int Execute()
		{
			Dir = Dir ?? Directory.GetCurrentDirectory();
			var configFile = Dir + "/config.xml";

			if (Start(Dir, configFile))
				return 0;

			var config = new FileInfo(configFile).DeserializeXml<Config>();

			Monitor.StartMonitor(Dir, config.ULearnCourseId);
			return 0;
		}
	}
}
