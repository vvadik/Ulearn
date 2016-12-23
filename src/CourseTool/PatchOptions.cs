using System;
using CommandLine;
using uLearn.Model.Edx;

namespace uLearn.CourseTool
{
	[Verb("down-olx", HelpText = "Download Edx course to olx directory from edx instance")]
	class DownloadOlxOption : AbstractOptions
	{
		public override void DoExecute()
		{
			Console.WriteLine("Profile {0}", Profile);
			var profile = Config.GetProfile(Profile);
			var credentials = Credentials.GetCredentials(Dir, Profile);
			EdxInteraction.Download(Dir, Config, profile.EdxStudioUrl, credentials);
		}
	}

	[Verb("up-olx", HelpText = "Upload Edx course from olx directory to edx instance")]
	class UploadOlxOption : AbstractOptions
	{
		public override void DoExecute()
		{
			Console.WriteLine("Profile {0}", Profile);
			var profile = Config.GetProfile(Profile);
			var credentials = Credentials.GetCredentials(Dir, Profile);
			Console.WriteLine("Loading OLX");
			var edxCourse = EdxCourse.Load(Dir + "/olx");
			EdxInteraction.Upload(edxCourse.CourseName, Config, profile.EdxStudioUrl, credentials);
		}
	}
	
	abstract class PatchOptions : AbstractOptions
	{
		[Option('s', "skip-existing", Default=false, HelpText = "If set, patch skips uLearn slides if edx slide with the same id exists already")]
		public bool SkipExistingGuids { get; set; }

		[Option('g', "guid", HelpText = "Specific guids to be patched separated by comma")]
		public string Guids { get; set; }

		[Option("down", HelpText = "Download course before patching")]
		public bool DownloadOlx { get; set; }

		[Option("patch", HelpText = "Patch course")]
		public bool PatchOlx { get; set; }

		[Option("up", HelpText = "Upload course after patching")]
		public bool UploadOlx { get; set; }

		[Option("full", HelpText = "combination of --down --patch --up")]
		public bool FullProcessingOlx { get; set; }

		public override void DoExecute()
		{
			if (!UploadOlx && !DownloadOlx && !PatchOlx && !FullProcessingOlx)
			{
				Console.WriteLine("Use the options: --up, --down, --patch, --full. Or --help for help");
				return;
			}
			Console.WriteLine("Profile {0}", Profile);
			var profile = Config.GetProfile(Profile);
			var credentials = Credentials.GetCredentials(Dir, Profile);

			if (DownloadOlx || FullProcessingOlx)
				EdxInteraction.Download(Dir, Config, profile.EdxStudioUrl, credentials);

			Console.WriteLine("Loading OLX");
			var edxCourse = EdxCourse.Load(Dir + "/olx");

			if (PatchOlx || FullProcessingOlx)
			{
				Console.WriteLine("Patching OLX...");
				Patch(new OlxPatcher(Dir + "/olx"), Config, profile, edxCourse);
				Console.WriteLine("Patched!");
			}

			if (UploadOlx || FullProcessingOlx)
				EdxInteraction.Upload(edxCourse.CourseName, Config, profile.EdxStudioUrl, credentials);
		}

		public abstract void Patch(OlxPatcher patcher, Config config, Profile profile, EdxCourse edxCourse);
	}
}