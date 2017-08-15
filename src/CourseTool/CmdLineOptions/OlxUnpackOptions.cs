using CommandLine;

namespace uLearn.CourseTool.CmdLineOptions
{
	[Verb("olx-unpack-targz", HelpText = "Unpack course.tar.gz to olx directory. Replaces all the old content of the olx directory. No patching!")]
	public class OlxUnpackOptions : AbstractOptions
	{
		[Option('t', "tar-gz", HelpText = "Filepath of course tar.gz file")]
		public string CourseTarGz { get; set; }

		public override void DoExecute()
		{
			EdxInteraction.ExtractEdxCourseArchive(Dir, Dir.GetSingleFile(CourseTarGz ?? "*.tar.gz"));
		}
	}
}