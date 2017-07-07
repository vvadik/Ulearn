using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Build.Evaluation;
using RunCsJob.Api;
using uLearn.Extensions;
using uLearn.NUnitTestRunning;
using uLearn.Properties;

namespace uLearn.Model.Blocks
{
	[XmlType("proj-exercise")]
	public class ProjectExerciseBlock : ExerciseBlock
	{
		public ProjectExerciseBlock()
		{
			StartupObject = "checking.CheckerRunner";
			HideExpectedOutputOnError = true;
			HideShowSolutionsButton = true;
			MaxScore = 50;
		}

		[XmlElement("csproj-file-path")]
		public string CsProjFilePath { get; set; }

		[XmlElement("startup-object")]
		public string StartupObject { get; set; }

		[XmlElement("user-code-file-name")]
		public string UserCodeFileName { get; set; }

		[XmlElement("solution-file")]
		public string SolutionFile { get; set; }

		[XmlElement("exclude-path-for-checker")]
		public string[] PathsToExcludeForChecker { get; set; }

		[XmlElement("nunit-test-class")]
		public string[] NUnitTestClasses { get; set; }

		[XmlElement("exclude-path-for-student")]
		public string[] PathsToExcludeForStudent { get; set; }
		public string ExerciseDir => Path.GetDirectoryName(CsProjFilePath).EnsureNotNull("csproj должен быть в поддиректории");

		public string CsprojFileName => Path.GetFileName(CsProjFilePath);
		[XmlIgnore]
		public DirectoryInfo SlideFolderPath { get; set; }

		public FileInfo StudentsZip => SlideFolderPath.GetFile(ExerciseDir + ".exercise.zip");

		public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
		{
			FillProperties(context);
			ExerciseInitialCode = ExerciseInitialCode ?? "// Вставьте сюда финальное содержимое файла " + UserCodeFileName;
			ExpectedOutput = ExpectedOutput ?? "";
			ValidatorName = string.Join(" ", LangId, ValidatorName);
			SlideFolderPath = context.Dir;
			var exercisePath = context.Dir.GetSubdir(ExerciseDir).FullName;
			if (context.ZippedProjectExercises.Add(exercisePath))
				CreateZipForStudent();

			CheckScoringGroup(context.SlideTitle, context.CourseSettings.Scoring);

			yield return this;

			if (!string.IsNullOrWhiteSpace(SolutionFile))
			{
				yield return new MdBlock("### Решение") { Hide = true };
				var content = File.ReadAllText(Path.Combine(exercisePath, SolutionFile));
				yield return new CodeBlock(content, LangId, LangVer) { Hide = true };
			}
		}

		private void CreateZipForStudent()
		{
			var directoryName = new DirectoryInfo(Path.Combine(SlideFolderPath.FullName, ExerciseDir));
			var zip = new LazilyUpdatingZip(directoryName, new[] { "checking", "bin", "obj" }, ReplaceCsproj, StudentsZip);
			zip.UpdateZip();
		}

		private byte[] ReplaceCsproj(FileInfo file)
		{
			if (!file.Name.Equals(CsprojFileName, StringComparison.InvariantCultureIgnoreCase))
				return null;
			return ProjModifier.ModifyCsproj(file, ProjModifier.RemoveCheckingFromCsproj);
		}

		public override string GetSourceCode(string code)
		{
			return code;
		}

		public override SolutionBuildResult BuildSolution(string userWrittenCodeFile)
		{
			var validator = ValidatorsRepository.Get(ValidatorName);
			return validator.ValidateSolution(userWrittenCodeFile, userWrittenCodeFile);
		}

		public override RunnerSubmission CreateSubmission(string submissionId, string code)
		{
			return new ProjRunnerSubmission
			{
				Id = submissionId,
				ZipFileData = GetZipBytesForChecker(code),
				ProjectFileName = CsprojFileName,
				Input = "",
				NeedRun = true
			};
		}

		private byte[] GetZipBytesForChecker(string code)
		{
			var directoryName = Path.Combine(SlideFolderPath.FullName, ExerciseDir);
			List<string> excluded = (PathsToExcludeForChecker ?? new string[0]).Concat(new[] { "bin/*", "obj/*" }).ToList();
			var exerciseDir = new DirectoryInfo(directoryName);
			return exerciseDir.ToZip(excluded, GetAdditionalFiles(code, exerciseDir, excluded));
		}

		private IEnumerable<FileContent> GetAdditionalFiles(string code, DirectoryInfo exerciseDir, List<string> excluded)
		{
			yield return new FileContent { Path = UserCodeFileName, Data = Encoding.UTF8.GetBytes(code) };

			var useNUnitLauncher = NUnitTestClasses != null;
			var startupObject = useNUnitLauncher ? typeof(NUnitTestRunner).FullName : StartupObject;

			yield return new FileContent
			{
				Path = CsprojFileName,
				Data = ProjModifier.ModifyCsproj(exerciseDir.GetFile(CsprojFileName), ModifyCsproj(excluded, startupObject, useNUnitLauncher))
			};

			if (useNUnitLauncher)
			{
				yield return new FileContent { Path = GetNUnitTestRunnerFilename(), Data = CreateTestLauncherFile() };
			}
		}

		private static string GetNUnitTestRunnerFilename()
		{
			return nameof(NUnitTestRunner)+".cs";
		}

		private static Action<Project> ModifyCsproj(List<string> excluded, string startupObject, bool addNUnitLauncher)
		{
			return p =>
			{
				ProjModifier.PrepareForChecking(p, startupObject, excluded);
				if (addNUnitLauncher)
					p.AddItem("Compile", GetNUnitTestRunnerFilename());
			};
		}

		private byte[] CreateTestLauncherFile()
		{
			var data = Resources.NUnitTestRunner;
			var oldTestFilter = "\"SHOULD_BE_REPLACED\"";
			var newTestFilter = string.Join(",", NUnitTestClasses.Select(x => $"\"{x}\""));
			var newData = data.Replace(oldTestFilter, newTestFilter);
			return Encoding.UTF8.GetBytes(newData);
		}
	}



}