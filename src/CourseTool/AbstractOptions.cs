using System;
using System.Diagnostics;
using System.IO;
using CommandLine;

namespace uLearn.CourseTool
{
	abstract class AbstractOptions
	{
		protected const string ExerciseUrlFormat = "/Course/{0}/LtiSlide/{1}";
		protected const string SolutionsUrlFormat = "/Course/{0}/AcceptedAlert/{1}";

		[Option('d', "dir", HelpText = "Working directory for the project")]
		public string Dir { get; set; }

		[Option('p', "profile", HelpText = "Profile used to work with Edx and uLearn")]
		public string Profile { get; set; }

		public bool Start(string dir, string configFile)
		{
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			if (!File.Exists(configFile))
			{
				var configTemplateFile = Path.Combine(Utils.GetRootDirectory(), "templates/config.xml");
				File.Copy(configTemplateFile, configFile);
				Process.Start("notepad", configFile);
				Console.WriteLine("Edit the config file {0} and run this option again.", configFile);
				return true;
			}
			return false;
		}

		public abstract int Execute();
	}
}