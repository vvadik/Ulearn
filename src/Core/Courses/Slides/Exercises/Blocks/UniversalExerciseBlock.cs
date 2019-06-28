using System;
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

		[XmlElement("dockerImageName")] // см. DockerImageNameRegex
		public string DockerImageName { get; set; }

		public static readonly Regex DockerImageNameRegex = new Regex("^[-_a-z.]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		
		[XmlElement("run")]
		public string RunCommand { get; set; } // см. RunCommandRegex
		
		public static readonly Regex RunCommandRegex = new Regex("^[-_a-z. ;|>]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		
		[XmlIgnore]
		public DirectoryInfo SlideDirectoryPath { get; set; }
		
		public DirectoryInfo ExerciseDirectory => new DirectoryInfo(Path.Combine(SlideDirectoryPath.FullName, ExerciseDirName));

		[XmlIgnore]
		public DirectoryInfo CourseDirectory { get; set; }
		
		public FileInfo UserCodeFile => ExerciseDirectory.GetFile(UserCodeFilePath);
		
		public DirectoryInfo UserCodeFileParentDirectory => UserCodeFile.Directory;
		
		public FileInfo CorrectSolutionFile => UserCodeFile;
		
		public string UserCodeFileNameWithoutExt => Path.GetFileNameWithoutExtension(UserCodeFilePath);
		
		private Regex WrongAnswersAndSolutionNameRegex => new Regex(new Regex("^") + UserCodeFileNameWithoutExt + new Regex("\\.(WrongAnswer|WA)\\..+"));

		public bool IsWrongAnswer(string name) => WrongAnswersAndSolutionNameRegex.IsMatch(name);
		
		public override string GetSourceCode(string code)
		{
			return code;
		}

		public override IEnumerable<SlideBlock> BuildUp(SlideBuildingContext context, IImmutableSet<string> filesInProgress)
		{
			if(!Language.HasValue)
				Language = LanguageHelpers.GuessByExtension(new FileInfo(UserCodeFilePath));
			SlideDirectoryPath = context.UnitDirectory;
			CourseDirectory = context.CourseDirectory;
			Validate();
			ExpectedOutput = ExpectedOutput ?? "";
			ExerciseInitialCode = ExerciseInitialCode ?? "// Вставьте сюда финальное содержимое файла " + UserCodeFilePath;
			yield return this;
			
			if (CorrectSolutionFile.Exists)
			{
				yield return new MarkdownBlock("### Решение") { Hide = true };
				yield return new CodeBlock(CorrectSolutionFile.ContentAsUtf8(), Language) { Hide = true };
			}
		}

		private void Validate()
		{
			if (DockerImageName == null)
				throw new ArgumentException("dockerImageName not specified");
			if (DockerImageName == null)
				throw new ArgumentException("run not specified");
			if(!DockerImageNameRegex.Match(DockerImageName).Success)
				throw new ArgumentException($"dockerImageName {DockerImageName}");
			if(!RunCommandRegex.Match(RunCommand).Success)
				throw new ArgumentException($"run {RunCommand}");
			if(!ExerciseDirectory.Exists)
				throw new ArgumentException($"exerciseDirName '{ExerciseDirName}' not exists");
			if(!UserCodeFile.Exists)
				throw new ArgumentException($"userCodeFile '{UserCodeFilePath}' not exists");
			foreach (var pathToIncludeForChecker in PathsToIncludeForChecker)
			{
				var di = new DirectoryInfo(Path.Combine(ExerciseDirectory.FullName, pathToIncludeForChecker));
				if(!IsDirectoryInside(CourseDirectory, di))
					throw new ArgumentException($"includePathForChecker '{pathToIncludeForChecker}' is not in subtree of directory with course.xml");
				if(!di.Exists)
					throw new ArgumentException($"includePathForChecker '{pathToIncludeForChecker}' not exists");
			}
		}

		public static bool IsDirectoryInside(DirectoryInfo baseDir, DirectoryInfo subDir)
		{
			var nBaseDir = NormalizePath(baseDir.FullName);
			var nSubDir = NormalizePath(subDir.FullName);
			return nSubDir.StartsWith(nBaseDir);
		}
		
		public static string NormalizePath(string path)
		{
			return Path.GetFullPath(new Uri(path).LocalPath)
				.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
				.ToUpperInvariant();
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