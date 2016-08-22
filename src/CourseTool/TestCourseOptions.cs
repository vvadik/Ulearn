using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

namespace uLearn.CourseTool
{
	[Verb("test", HelpText = "Run tests on course")]
	class TestCourseOptions : AbstractOptions
	{
		public override void DoExecute()
		{
			var ulearnDir = new DirectoryInfo(string.Format("{0}/{1}", Dir, Config.ULearnCourseId));
			Console.WriteLine("Loading Ulearn course from {0}", ulearnDir.Name);
			var course = new CourseLoader().LoadCourse(ulearnDir);
			var validator = new CourseValidator(course, AppDomain.CurrentDomain.BaseDirectory);
			validator.InfoMessage += m => Write(ConsoleColor.Gray, m);
			var errors = new List<string>();
			validator.Error += m => {
				Write(ConsoleColor.Red, m);
				errors.Add(m);
			};
			validator.ValidateExercises();
			validator.ValidateVideos();
			if (errors.Any())
			{
				Console.WriteLine("Done! There are errors:");
				foreach (var error in errors)
				{
					Write(ConsoleColor.Red, error);
				}
			}
			else
				Console.WriteLine("OK! No errors found");
			Console.WriteLine("Press any key...");
			Console.ReadLine();
		}

		private void Write(ConsoleColor color, string message)
		{
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			try
			{
				Console.WriteLine(message);
			}
			finally
			{
				Console.ForegroundColor = oldColor;
			}
		}
	}
}