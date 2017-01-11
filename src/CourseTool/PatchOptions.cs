using System;
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

		[Option("targz", HelpText = "Course tar.gz file from edx (will be guessed if not specified)")]
		public string CourseTarGz { get; set; }

		public override void DoExecute()
		{
			Console.WriteLine("Profile {0}", Profile);
			var profile = Config.GetProfile(Profile);

			var tarGzPath = Dir.GetSingleFile(CourseTarGz ?? "*.tar.gz");
			EdxInteraction.ExtractEdxCourseArchive(Dir, tarGzPath);

			Console.WriteLine("Loading OLX");
			var edxCourse = EdxCourse.Load(Dir + "/olx");
			Console.WriteLine("Patching OLX...");
			Patch(new OlxPatcher(Dir + "/olx"), Config, profile, edxCourse);
			Console.WriteLine("Patched!");
			EdxInteraction.CreateEdxCourseArchive(Dir, Config.ULearnCourseId);
		}

		public abstract void Patch(OlxPatcher patcher, Config config, Profile profile, EdxCourse edxCourse);
	}
}