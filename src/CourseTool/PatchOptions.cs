using System;
using System.IO;
using System.Linq;
using CommandLine;
using uLearn.Model.Edx;

namespace uLearn.CourseTool
{
	abstract class PatchOptions : AbstractOptions
	{
		[Option('s', "skip-existing", Default=false, HelpText = "If set, patch skips uLearn slides if edx slide with the same id exists already")]
		public bool SkipExistingGuids { get; set; }

		[Option('g', "guid", HelpText = "Specific guids to be patched separated by comma")]
		public string Guids { get; set; }

		[Option("up", HelpText = "Upload Only — do not download course before patching. Usefull after manual edit of olx dir")]
		public bool UploadOnly { get; set; }

		[Option("down", HelpText = "Download Only — do not upload course after patching. Usefull to download results of EdxStudio edits")]
		public bool DownloadOnly { get; set; }

		public override int Execute()
		{
			Dir = Dir ?? Directory.GetCurrentDirectory();
			Profile = Profile ?? "default";

			var configFile = Dir + "/config.xml";

			if (Start(Dir, configFile))
				return 0;

			var config = new FileInfo(configFile).DeserializeXml<Config>();
			var profile = CourseTool.Profile.GetProfile(config, Profile);
			var credentials = Credentials.GetCredentials(Dir, Profile);

			if (DownloadOnly || !UploadOnly)
				DownloadManager.Download(Dir, config, profile.EdxStudioUrl, credentials);

			Console.WriteLine("Loading OLX");
			var edxCourse = EdxCourse.Load(Dir + "/olx");

			Console.WriteLine("Patching OLX...");
			Patch(new OlxPatcher(Dir + "/olx"), config, profile, edxCourse);
			Console.WriteLine("Patched!");

			if(UploadOnly || !DownloadOnly)
				DownloadManager.Upload(Dir, edxCourse.CourseName, config, profile.EdxStudioUrl, credentials);
			return 0;
		}

		public abstract void Patch(OlxPatcher patcher, Config config, Profile profile, EdxCourse edxCourse);
	}
}