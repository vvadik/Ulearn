using System.IO;
using CommandLine;
using uLearn.Model.Edx;

namespace uLearn.CourseTool
{
	abstract class PatchOptions : AbstractOptions
	{
		[Option('d', "dir", HelpText = "Working directory for the project")]
		public string Dir { get; set; }

		[Option('r', "replace", HelpText = "If set, patch replaces Edx slides on uLearn slides with same guid")]
		public bool ReplaceExisting { get; set; }

		[Option('g', "guid", HelpText = "Specific guids to be patched separated by comma")]
		public string Guids { get; set; }

		public override int Execute()
		{
			Dir = Dir ?? Directory.GetCurrentDirectory();
			var configFile = Dir + "/config.xml";

			if (Start(Dir, configFile))
				return 0;

			var config = new FileInfo(configFile).DeserializeXml<Config>();
			var credentials = Credentials.GetCredentials(Dir);

			DownloadManager.Download(Dir, config, credentials);

			var edxCourse = EdxCourse.Load(Dir + "/olx");

			Patch(new OlxPatcher(Dir + "/olx"), config, edxCourse);

			DownloadManager.Upload(Dir, edxCourse.CourseName, config, credentials);
			return 0;
		}

		public abstract void Patch(OlxPatcher patcher, Config config, EdxCourse edxCourse);
	}
}