using CommandLine;

namespace uLearnToEdx
{
	[Verb("convert", HelpText = "Convert uLearn course to Edx course.")]
	class ConvertOptions
	{
		[Option('d', "dir", HelpText = "Working directory for the project", Required = true)]
		public string Dir { get; set; }

		[Option('i', "input", HelpText = "Directory with uLearn course to be converted", Required = true)]
		public string InputDir { get; set; }

		[Option('v', "video", HelpText = "Json file with information about video used in the course")]
		public string VideoJson { get; set; }
	}

	[Verb("patch", HelpText = "Patch Edx course with new slides or videos.")]
	class PatchOptions
	{
		[Option('d', "dir", HelpText = "Working directory for the project", Required = true)]
		public string Dir { get; set; }

		[Option('r', "replace", HelpText = "If set, patch replaces Edx slides on uLearn slides with same guid")]
		public bool ReplaceExisting { get; set; }

		[Option('g', "guid", HelpText = "Specific guids to be patched separated by comma")]
		public string Guids { get; set; }
	}

	[Verb("start", HelpText = "Perform initial setup for Edx course.")]
	internal class StartOptions
	{
		[Option('d', "dir", HelpText = "Working directory for the project", Required = true)]
		public string Dir { get; set; }
	}
}
