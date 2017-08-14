using CommandLine;

namespace uLearn.CourseTool.CmdLineOptions
{
	[Verb("olx-pack-targz", HelpText = "Create <CourseName>.tar.gz from olx directory")]
	public class OlxPackOptions : AbstractOptions
	{
		public override void DoExecute()
		{
			EdxInteraction.CreateEdxCourseArchive(Dir, Config.ULearnCourseId);
		}
	}
}