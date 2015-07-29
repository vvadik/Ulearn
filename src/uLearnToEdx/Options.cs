using CommandLine;

namespace uLearnToEdx
{
	[Verb("convert", HelpText = "Convert uLearn course to Edx course.")]
	class ConvertOptions
	{
		[Option('i', HelpText = "", Required = true)]
		public string InputDir { get; set; }

		[Option('v')]
		public string VideoJson { get; set; }

		[Option('o', HelpText = "")]
		public string OutputDir { get; set; }
	}

	[Verb("patch", HelpText = "Patch Edx course with new slides or videos.")]
	class PatchOptions
	{
		[Option('i')]
		public string InputDir { get; set; }

		[Option('v')]
		public string VideoJson { get; set; }
	}

	[Verb("start", HelpText = "Perform initial setup for Edx course.")]
	internal class StartOptions
	{
		[Option('d', "dir", Required = true)]
		public string Dir { get; set; }

		[Option('o', "org", Required = true)]
		public string Organization { get; set; }

		[Option('n', "course_number", Required = true)]
		public string CourseNumber { get; set; }

		[Option('r', "course_run", Required = true)]
		public string CourseRun { get; set; }

		[Option('a', "advanced_modules", Required = true)]
		public string AdvancedModules { get; set; }

		[Option('l', "lti_passports", Required = true)]
		public string LtiPassports { get; set; }

		[Option('h', "hostname", Required = true)]
		public string Hostname { get; set; }

		[Option('p', "port", Required = true)]
		public int Port { get; set; }

		[Option('e', "exercise_url", Required = true)]
		public string ExerciseUrl { get; set; }

		[Option('s', "solutions_url", Required = true)]
		public string SolutionsUrl { get; set; }
	}
}
