using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
		public string ExerciseDirName { get; set; } // Если нет поддиректории src, содержимое для чекера помещается в архив в папку src, в архиве дял студента такого не происходит
		
		[XmlElement("userCodeFile")]
		public string UserCodeFilePath { get; set; }
		
		[XmlElement("excludePathForChecker")]
		public string[] PathsToExcludeForChecker { get; set; }
		
		[XmlElement("includePathForChecker")]
		public string[] PathsToIncludeForChecker { get; set; } // Пути до папок относительно ExerciseDirName. Перетирают файлы из exerciseDirName

		[XmlElement("excludePathForStudent")]
		public string[] PathsToExcludeForStudent { get; set; } // Шаблоны путей до файлов внутри ExerciseDirName. Нужно вручную учитывать папку src, если путь от корня
		
		[XmlElement("studentZipIsCompilable")]
		public bool StudentZipIsCompilable { get; set; } = true;
		
		[XmlElement("correctSolutionFileName")]
		public string CorrectSolutionFileName { get; set; }
		
		[XmlElement("dockerImageName")]
		public string DockerImageName { get; set; }
		
		[XmlElement("run")]
		public string RunCommand { get; set; }
		
		[XmlIgnore]
		public DirectoryInfo SlideDirectoryPath { get; set; }
		
		public DirectoryInfo ExerciseDirectory => new DirectoryInfo(Path.Combine(SlideDirectoryPath.FullName, ExerciseDirName));
		
		public FileInfo UserCodeFile => ExerciseDirectory.GetFile(UserCodeFilePath);
		
		public DirectoryInfo UserCodeFileParentDirectory => UserCodeFile.Directory;
		
		public FileInfo CorrectSolutionFile => UserCodeFileParentDirectory.GetFile(CorrectSolutionFileName);
		
		public string UserCodeFileNameWithoutExt => Path.GetFileNameWithoutExtension(UserCodeFilePath);
		
		private Regex WrongAnswersAndSolutionNameRegex => new Regex(new Regex("^") + UserCodeFileNameWithoutExt + new Regex("\\.(WrongAnswer|WA)\\..+"));

		public bool IsWrongAnswer(string name) => WrongAnswersAndSolutionNameRegex.IsMatch(name);
		
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
			return new CommandRunnerSubmission
			{
				Id = submissionId,
				ZipFileData = GetZipBytesForChecker(code),
// ReSharper disable once PossibleInvalidOperationException
				Language = Language.Value,
				DockerImageName = DockerImageName,
				RunCommand = RunCommand,
			};
		}

		private byte[] GetZipBytesForChecker(string code)
		{
			var excluded = (PathsToExcludeForChecker ?? new string[0])
				.Concat(new[] { "*.Initial.*", "*.WrongAnswer.*", "*.WA.*" })
				.ToList();

			var toUpdateDirectories = PathsToIncludeForChecker
				.Select(pathToInclude => new DirectoryInfo(Path.Combine(ExerciseDirectory.FullName, pathToInclude)));

			var toUpdate = GetCodeFile(code).ToList();
			var hasSrcDir = ExerciseDirectory.EnumerateDirectories("src").Any();
			var zipBytes = ExerciseDirectory.ToZip(excluded, toUpdate, toUpdateDirectories, hasSrcDir ? null : "src");

			log.Info($"Собираю zip-архив для проверки: zip-архив собран, {zipBytes.Length} байтов");
			return zipBytes;
		}
		
		private IEnumerable<FileContent> GetCodeFile(string code)
		{
			yield return new FileContent { Path = UserCodeFilePath, Data = Encoding.UTF8.GetBytes(code) };
		}
		
		public byte[] GetZipBytesForStudent()
		{
			var excluded = (PathsToExcludeForStudent ?? new string[0])
				.Concat(new[] { "*.Initial.*", "*.WrongAnswer.*", "*.WA.*", "checking/*" })
				.ToList();
			
			var toUpdate = ReplaceWithInitialFiles().ToList();
			
			var zipBytes = ExerciseDirectory.ToZip(excluded, toUpdate);
			log.Info($"Собираю zip-архив для студента: zip-архив собран, {zipBytes.Length} байтов");
			return zipBytes;
		}

		private IEnumerable<FileContent> ReplaceWithInitialFiles()
		{
			var initialFiles = ExerciseDirectory.GetFiles("*.Initial.*", SearchOption.AllDirectories);
			foreach (var initialFile in initialFiles)
			{
				var relativeFilePath = initialFile.GetRelativePath(ExerciseDirectory.FullName);
				var notInitial = relativeFilePath.Replace(".Initial", "");
				yield return new FileContent { Path = notInitial, Data = initialFile.ReadAllContent() };
			}
		}
	}
}