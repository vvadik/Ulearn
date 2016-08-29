using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using RunCsJob.Api;

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
		public string ExerciseDir => Path.GetDirectoryName(CsProjFilePath).EnsureNotNull("csproj должен быть в поддиректории");

		public string CsprojFileName => Path.GetFileName(CsProjFilePath);
		[XmlIgnore]
		public string SlideFolderPath { get; private set; }

		public FileInfo StudentsZip => new DirectoryInfo(SlideFolderPath).GetFile(ExerciseDir + ".exercise.zip");

		public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
		{
			FillProperties(context);
			ExerciseInitialCode = ExerciseInitialCode ?? "// Вставьте сюда финальное содержимое файла " + UserCodeFileName;
			ExpectedOutput = ExpectedOutput ?? "";
			ValidatorName = string.Join(" ", LangId, ValidatorName);
			SlideFolderPath = context.Dir.FullName;
			var exercisePath = context.Dir.GetSubdir(ExerciseDir).FullName;
			if (context.ZippedProjectExercises.Add(exercisePath))
				CreateZipForStudent();
			yield return this;
		}

		private void CreateZipForStudent()
		{
			var directoryName = new DirectoryInfo(Path.Combine(SlideFolderPath, ExerciseDir));
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

		public override SolutionBuildResult BuildSolution(string code)
		{
			var validator = ValidatorsRepository.Get(ValidatorName);
			return validator.ValidateSolution(code);
		}

		public override RunnerSubmition CreateSubmition(string submitionId, string code)
		{
			return new ProjRunnerSubmition
			{
				Id = submitionId,
				ZipFileData = GetZipBytesForChecker(code),
				ProjectFileName = CsprojFileName,
				Input = "",
				NeedRun = true
			};
		}

		private byte[] GetZipBytesForChecker(string code)
		{
			var directoryName = Path.Combine(SlideFolderPath, ExerciseDir);
			var excluded = (PathsToExcludeForChecker ?? new string[0]).Concat(new[] { "bin/*", "obj/*" }).ToList();
			var exerciseDir = new DirectoryInfo(directoryName);
			return exerciseDir.ToZip(excluded,
				new[]
				{
					new FileContent { Path = UserCodeFileName, Data = Encoding.UTF8.GetBytes(code) },
					new FileContent { Path = CsprojFileName,
						Data = ProjModifier.ModifyCsproj(exerciseDir.GetFile(CsprojFileName), 
						p => ProjModifier.PrepareForChecking(p, this)) }
				});
		}
	}
}