using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Ulearn.Common;
using Ulearn.Core.RunCheckerJobApi;

namespace Ulearn.Core.Courses.Slides.Exercises.Blocks
{
	[XmlType("exercise.polygon")]
	public class PolygonExerciseBlock : UniversalExerciseBlock
	{
		public static Language[] Languages => new[]
		{
			Common.Language.Python3,
			Common.Language.JavaScript,
			Common.Language.Cpp
		};
		
		public override string DockerImageName => "algorithms-sandbox";
		public override bool NoStudentZip => true;
		public override string UserCodeFilePath => "solution.any";
		public override Language? Language => Common.Language.Any;
		public override string RunCommand => "python3 main.py";
		public override string Region => "Task";
		public int MsPerTest { get; set; }
		public override string[] PathsToExcludeForChecker => new[]
		{
			"files",
			"scripts",
			"solutions",
			"statements",
			"statements-sections",
			"check.exe",
			"doall.bat",
			"problem.xml",
			"tags",
			"wipe.bat"
		};
		
		public RunnerSubmission CreateSubmission(string submissionId, string code, Language language)
		{
			var submission = base.CreateSubmission(submissionId, code);
			if (!(submission is CommandRunnerSubmission commandRunnerSubmission))
				return submission;
			
			commandRunnerSubmission.RunCommand = RunCommandWithArguments(language);
			
			return commandRunnerSubmission;
		}

		private string RunCommandWithArguments(Language language)
		{
			return $"{RunCommand} {language} {MsPerTest}";
		}
	}
}