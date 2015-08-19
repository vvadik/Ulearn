using System;
using System.Diagnostics;
using System.IO;

namespace uLearn.CourseTool
{
	abstract class AbstractOptions
	{
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