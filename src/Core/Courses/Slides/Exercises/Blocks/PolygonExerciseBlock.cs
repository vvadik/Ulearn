using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Ulearn.Common;
using Ulearn.Core.RunCheckerJobApi;

namespace Ulearn.Core.Courses.Slides.Exercises.Blocks
{
	[XmlType("exercise.polygon")]
	public class PolygonExerciseBlock : UniversalExerciseBlock
	{
		public static Dictionary<Language, LanguageLaunchInfo> LanguagesInfo = new Dictionary<Language, LanguageLaunchInfo>
		{
			[Common.Language.Java] = new LanguageLaunchInfo
			{
				Compiler = "Java 1.8",
				CompileCommand = "javac {source}",
				RunCommand = "java {compilation-result-file}"
			},
			[Common.Language.Python3] = new LanguageLaunchInfo
			{
				Compiler = "Python 3.8",
				RunCommand = "python3.8 {source}"
			},
			[Common.Language.JavaScript] = new LanguageLaunchInfo
			{
				Compiler = "Node.js v14.15",
				RunCommand = "node {source}"
			} ,
			[Common.Language.Cpp] = new LanguageLaunchInfo
			{
				Compiler = "G++ 10.2",
				CompileCommand = "g++ -o {name-executable-file} -O2 {source}",
				RunCommand = "./{name-executable-file}"
			} ,
			[Common.Language.CSharp] = new LanguageLaunchInfo
			{
				Compiler = ".Net SDK 5.0",
				CompileCommand = "dotnet build",
				RunCommand = "./bin/Debug/net5.0/app"
			},
			[Common.Language.C] = new LanguageLaunchInfo
			{
				Compiler = "CC 10.2",
				CompileCommand = "cc -o {name-executable-file} -O {source}",
				RunCommand = "./{name-executable-file}"
			}
		};
		
		public override string DockerImageName => "algorithms-sandbox";
		public override bool NoStudentZip => true;
		public override string UserCodeFilePath => "Program.any";
		public override Language? Language => Common.Language.Any;
		public override string RunCommand => "python3.8 main.py";
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
			commandRunnerSubmission.TimeLimit = (int)Math.Ceiling(TimeLimit / 1000d);
			
			return commandRunnerSubmission;
		}

		private string RunCommandWithArguments(Language language)
		{
			return $"{RunCommand} {language} {MsPerTest}";
		}
	}
}