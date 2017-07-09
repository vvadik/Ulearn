using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RunCsJob.Api;
using uLearn.Extensions;

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

		[XmlElement("exclude-path-for-checker")]
		public string[] PathsToExcludeForChecker { get; set; }

		[XmlElement("exclude-path-for-student")]
		public string[] PathsToExcludeForStudent { get; set; }
		public string ExerciseDirName => Path.GetDirectoryName(CsProjFilePath).EnsureNotNull("csproj должен быть в поддиректории");

		public string CsprojFileName => Path.GetFileName(CsProjFilePath);

		public FileInfo CsprojFile => ExerciseFolder.GetFile(CsprojFileName);

		public FileInfo UserCodeFile => ExerciseFolder.GetFile(UserCodeFileName);

		public FileInfo SolutionFile => ExerciseFolder.GetFile(CorrectSolutionFileName);

		public DirectoryInfo ExerciseFolder => new DirectoryInfo(Path.Combine(SlideFolderPath.FullName, ExerciseDirName));

		public string UserCodeFileNameWithoutExt => Path.GetFileNameWithoutExtension(UserCodeFileName);

		public string WrongAnswerPathRegexPattern => $"(.*){UserCodeFileNameWithoutExt}\\.WrongAnswer\\.(.+)\\.cs";

		public string CorrectSolutionFileName => $"{UserCodeFileNameWithoutExt}.Solution.cs";

		[XmlIgnore]
		public DirectoryInfo SlideFolderPath { get; set; }

		public FileInfo StudentsZip => SlideFolderPath.GetFile(ExerciseDirName + ".exercise.zip");

		public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
		{
			FillProperties(context);
			ExerciseInitialCode = ExerciseInitialCode ?? "// Вставьте сюда финальное содержимое файла " + UserCodeFileName;
			ExpectedOutput = ExpectedOutput ?? "";
			ValidatorName = string.Join(" ", LangId, ValidatorName);
			SlideFolderPath = context.Dir;
			var exercisePath = context.Dir.GetSubdir(ExerciseDirName).FullName;
			if (context.ZippedProjectExercises.Add(exercisePath))
				CreateZipForStudent();

			CheckScoringGroup(context.SlideTitle, context.CourseSettings.Scoring);

			yield return this;

			if (SolutionFile.Exists)
			{
				yield return new MdBlock("### Решение") { Hide = true };
				yield return new CodeBlock(SolutionFile.ContentAsUtf8(), LangId, LangVer) { Hide = true };
			}
		}

		private void CreateZipForStudent()
		{
			var zip = new LazilyUpdatingZip(
				ExerciseFolder, 
				new[] { "checking", "bin", "obj" },
				new[] { CorrectSolutionFileName },
				WrongAnswerPathRegexPattern, ReplaceCsproj, StudentsZip);
			zip.UpdateZip();
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
			List<string> excluded = (PathsToExcludeForChecker ?? new string[0]).Concat(new[] { "bin/*", "obj/*" }).ToList();

			return ExerciseFolder.ToZip(excluded,
				new[]
				{
					new FileContent { Path = UserCodeFileName, Data = Encoding.UTF8.GetBytes(code) },
					new FileContent { Path = CsprojFileName, // todo paatrofimov: проблема читаемости - в FileContent.Path везде пишется Name
						Data = ProjModifier.ModifyCsproj(
							CsprojFile,
							p => ProjModifier.PrepareForCheckingUserCode(p, this, excluded))
					}
				});
		}
	}
}