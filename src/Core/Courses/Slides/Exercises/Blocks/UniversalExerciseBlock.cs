using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using log4net;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.RunCheckerJobApi;

namespace Ulearn.Core.Courses.Slides.Exercises.Blocks
{
	[XmlType("exercise.universal")]
	public class UniversalExerciseBlock: AbstractExerciseBlock
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(UniversalExerciseBlock));
		
		[XmlAttribute("exerciseDirName")]
		public string ExerciseDirName { get; set; }
		
		[XmlElement("userCodeFile")]
		public string UserCodeFilePath { get; set; }
		
		[XmlElement("excludePathForChecker")]
		public string[] PathsToExcludeForChecker { get; set; }

		[XmlElement("excludePathForStudent")]
		public string[] PathsToExcludeForStudent { get; set; }
		
		[XmlElement("studentZipIsCompilable")]
		public bool StudentZipIsCompilable { get; set; } = true;
		
		[XmlElement("correctSolutionFileName")]
		public string CorrectSolutionFileName { get; set; }
		
		[XmlIgnore]
		public DirectoryInfo SlideDirectoryPath { get; set; }
		
		public DirectoryInfo ExerciseFolder => new DirectoryInfo(Path.Combine(SlideDirectoryPath.FullName, ExerciseDirName));
		
		public FileInfo UserCodeFile => ExerciseFolder.GetFile(UserCodeFilePath);
		
		public DirectoryInfo UserCodeFileParentDirectory => UserCodeFile.Directory;
		
		public FileInfo CorrectSolutionFile => UserCodeFileParentDirectory.GetFile(CorrectSolutionFileName);
		
		public override string GetSourceCode(string code)
		{
			return code;
		}

		public override IEnumerable<SlideBlock> BuildUp(SlideBuildingContext context, IImmutableSet<string> filesInProgress)
		{
			if (!Language.HasValue)
				Language = LanguageHelpers.GuessByExtension(new FileInfo(UserCodeFilePath));
			SlideDirectoryPath = context.UnitDirectory;
			ExerciseInitialCode = ExerciseInitialCode ?? "// Вставьте сюда финальное содержимое файла " + UserCodeFilePath;
			yield return this;
			
			if (CorrectSolutionFile.Exists)
			{
				yield return new MarkdownBlock("### Решение") { Hide = true };
				yield return new CodeBlock(CorrectSolutionFile.ContentAsUtf8(), Language) { Hide = true };
			}
		}

		public override SolutionBuildResult BuildSolution(string userWrittenCode)
		{
			return new SolutionBuildResult(userWrittenCode);
		}

		public override RunnerSubmission CreateSubmission(string submissionId, string code)
		{
			return new ZipRunnerSubmission
			{
				Id = submissionId,
				ZipFileData = GetZipBytesForChecker(code),
				Input = "",
// ReSharper disable once PossibleInvalidOperationException
				Language = Language.Value,
				NeedRun = true
			};
		}

		private byte[] GetZipBytesForChecker(string code)
		{
			var excluded = (PathsToExcludeForChecker ?? new string[0])
				.Concat(new[] { "bin/*", "obj/*" })
				.ToList();
			
			log.Info("Собираю zip-архив для проверки: получаю список дополнительных файлов");
			var toUpdate = GetAdditionalFiles(code).ToList();
			log.Info($"Собираю zip-архив для проверки: дополнительные файлы [{string.Join(", ", toUpdate.Select(c => c.Path))}]");
			
			var zipBytes = ExerciseFolder.ToZip(excluded, toUpdate);
			log.Info($"Собираю zip-архив для проверки: zip-архив собран, {zipBytes.Length} байтов");
			return zipBytes;
		}

		private IEnumerable<FileContent> GetAdditionalFiles(string code)
		{
			yield return new FileContent { Path = UserCodeFilePath, Data = Encoding.UTF8.GetBytes(code) };
		}
	}
}