using CommandLine;

namespace CourseToolHotReloader.DirectoryWorkers
{
	internal class Options
	{
		[Option("sendfullarchive", Required = false, Hidden = true, Default = true, HelpText = "send full archive")]
		public bool SendFullArchive { get; set; }

		[Option("login", Required = true, HelpText = "Enter login of ulearn account")]
		public string Login { get; set; }

		[Option("password", Required = true, HelpText = "Enter password of ulearn account")]
		public string Password { get; set; }

		[Option("courseId", Required = true, HelpText = "Set your temp course id or enter exist temp course id")]
		public string CourseId { get; set; }

		[Option("courseIdAlreadyExist", Required = false, Default = false, HelpText = "If your temp course already exist. And you want contine working.")]
		public bool CourseIdAlreadyExist { get; set; }
	}
}