using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
		private static readonly string anySolutionNameRegex = new Regex("(.+)\\.Solution\\.cs").ToString();
		private static readonly string anyWrongAnswerNameRegex = new Regex("(.+)\\.WrongAnswer\\.(.+)\\.cs").ToString();

		public static bool IsAnyWrongAnswerOrAnySolution(string name) => Regex.IsMatch(name, anyWrongAnswerNameRegex) || Regex.IsMatch(name, anySolutionNameRegex);
		public static bool IsAnySolution(string name) => Regex.IsMatch(name, anySolutionNameRegex);

		public static string SolutionFilenameToUserCodeFilename(string solutionFilename)
		{
			var userCodeFilenameWithoutExt = solutionFilename.Split(new[] { ".Solution.cs" }, StringSplitOptions.RemoveEmptyEntries).First();
			return $"{userCodeFilenameWithoutExt}.cs";
		}

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

		[XmlElement("user-code-file-path")]
		public string UserCodeFilePath { get; set; }

		[XmlElement("user-code-file-name")]
		public string ObsoleteUserCodeFileName
		{
			get => null;
			set => UserCodeFilePath = value;
		}

		[XmlElement("exclude-path-for-checker")]
		public string[] PathsToExcludeForChecker { get; set; }

		[XmlElement("nunit-test-class")]
		public string[] NUnitTestClasses { get; set; }

		[XmlElement("exclude-path-for-student")]
		public string[] PathsToExcludeForStudent { get; set; }

		[XmlElement("student-zip-is-buildable")]
		public bool StudentZipIsBuildable { get; set; } = true;

		public string ExerciseDirName => Path.GetDirectoryName(CsProjFilePath).EnsureNotNull("csproj должен быть в поддиректории");

		public string CsprojFileName => Path.GetFileName(CsProjFilePath);

		public FileInfo CsprojFile => ExerciseFolder.GetFile(CsprojFileName);

		public FileInfo UserCodeFile => ExerciseFolder.GetFile(UserCodeFilePath);

		public DirectoryInfo UserCodeFileParentDirectory => UserCodeFile.Directory;

		public FileInfo CorrectSolutionFile => UserCodeFileParentDirectory.GetFile(CorrectSolutionFileName);

		public DirectoryInfo ExerciseFolder => new DirectoryInfo(Path.Combine(SlideFolderPath.FullName, ExerciseDirName));

		public string UserCodeFileNameWithoutExt => Path.GetFileNameWithoutExtension(UserCodeFilePath);

		public string CorrectSolutionFileName => $"{UserCodeFileNameWithoutExt}.Solution.cs";

		public string CorrectSolutionPath => CorrectSolutionFile.GetRelativePath(ExerciseFolder.FullName);

		private string WrongAnswersAndSolutionNameRegexPattern => UserCodeFileNameWithoutExt + new Regex("\\.(.+)\\.cs");

		[XmlIgnore]
		public DirectoryInfo SlideFolderPath { get; set; }

		public FileInfo StudentsZip => SlideFolderPath.GetFile(ExerciseDirName + ".exercise.zip");

		public bool IsWrongAnswer(string name) => Regex.IsMatch(name, WrongAnswersAndSolutionNameRegexPattern) && !IsCorrectSolution(name);

		public bool IsCorrectSolution(string name) => name.Equals(CorrectSolutionFileName, StringComparison.InvariantCultureIgnoreCase);

		public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
		{
			FillProperties(context);
			ExerciseInitialCode = ExerciseInitialCode ?? "// Вставьте сюда финальное содержимое файла " + UserCodeFilePath;
			ExpectedOutput = ExpectedOutput ?? "";
			ValidatorName = string.Join(" ", LangId, ValidatorName);
			SlideFolderPath = context.Dir;
			var exercisePath = context.Dir.GetSubdir(ExerciseDirName).FullName;
			if (context.ZippedProjectExercises.Add(exercisePath))
				CreateZipForStudent();

			CheckScoringGroup(context.SlideTitle, context.CourseSettings.Scoring);

			yield return this;

			if (CorrectSolutionFile.Exists)
			{
				yield return new MdBlock("### Решение") { Hide = true };
				yield return new CodeBlock(CorrectSolutionFile.ContentAsUtf8(), LangId, LangVer) { Hide = true };
			}
		}

		private void CreateZipForStudent()
		{
			var zip = new LazilyUpdatingZip(
				ExerciseFolder,
				new[] { "checking", "bin", "obj" },
				IsAnyWrongAnswerOrAnySolution,
				ReplaceCsproj, StudentsZip);
			ResolveCsprojLinks();
			zip.UpdateZip();
		}

		private void ResolveCsprojLinks()
		{
			ProjModifier.ModifyCsproj(ExerciseFolder.GetFile(CsprojFileName), ProjModifier.ResolveLinks);
		}

		private byte[] ReplaceCsproj(FileInfo file)
		{
			if (!file.Name.Equals(CsprojFileName, StringComparison.InvariantCultureIgnoreCase))
				return null;
			return ProjModifier.ModifyCsproj(file, proj => ProjModifier.PrepareForStudentZip(proj, this));
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

		public byte[] GetZipBytesForChecker(string code)
		{
			var excluded = (PathsToExcludeForChecker ?? new string[0])
				.Concat(new[] { "bin/*", "obj/*" })
				.ToList();
			ResolveCsprojLinks();
			return ExerciseFolder.ToZip(excluded, GetAdditionalFiles(code, ExerciseFolder, excluded));
		}

		private IEnumerable<FileContent> GetAdditionalFiles(string code, DirectoryInfo exerciseDir, List<string> excluded)
		{
			yield return new FileContent { Path = UserCodeFilePath, Data = Encoding.UTF8.GetBytes(code) };

			var useNUnitLauncher = NUnitTestClasses != null;
			StartupObject = useNUnitLauncher ? typeof(NUnitTestRunner).FullName : StartupObject;

			yield return new FileContent
			{
				Path = CsprojFileName,
				Data = ProjModifier.ModifyCsproj(exerciseDir.GetFile(CsprojFileName), ModifyCsproj(excluded, useNUnitLauncher))
			};

			if (useNUnitLauncher)
			{
				yield return new FileContent { Path = GetNUnitTestRunnerFilename(), Data = CreateTestLauncherFile() };
			}
		}

		private static string GetNUnitTestRunnerFilename()
		{
			return nameof(NUnitTestRunner) + ".cs";
		}

		private Action<Project> ModifyCsproj(List<string> excluded, bool addNUnitLauncher)
		{
			return p =>
			{
				ProjModifier.PrepareForCheckingUserCode(p, this, excluded);
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