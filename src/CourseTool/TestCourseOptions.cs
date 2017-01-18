using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using RunCsJob;

namespace uLearn.CourseTool
{
	[Verb("test", HelpText = "Run tests on course")]
	class TestCourseOptions : AbstractOptions
	{
		[Option('s', "slide", HelpText = "SlideId to test only one specific slide")]
		public string SlideId { get; set; }

		public override void DoExecute()
		{
			var ulearnDir = new DirectoryInfo(string.Format("{0}/{1}", Dir, Config.ULearnCourseId));
			Console.Write("Loading Ulearn course from {0} ... ", ulearnDir.Name);
			var sw = Stopwatch.StartNew();
			var course = new CourseLoader().LoadCourse(ulearnDir);
			Console.WriteLine(sw.ElapsedMilliseconds + " ms");
			var slides = course.Slides;
			if (SlideId != null)
			{
				slides = course.Slides.Where(s => s.Id == Guid.Parse(SlideId)).ToArray();
				Console.WriteLine("Only slide " + SlideId);
			}
			
			var validator = new CourseValidator(slides, new SandboxRunnerSettings());
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