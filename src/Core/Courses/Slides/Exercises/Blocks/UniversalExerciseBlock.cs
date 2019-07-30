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
using Ulearn.Core.Model;
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
		
		[XmlElement("region")]
		public string Region { get; set; } // Студент видит в консоли и редактирует регион с этим именем. <prefix>region label <...> <prefix>rendregion label. Должен быть как в решении, так и в файле заглушке.
		
		[XmlElement("excludePathForChecker")]
		public string[] PathsToExcludeForChecker { get; set; }
		
		[XmlElement("includePathForChecker")]
		public string[] PathsToIncludeForChecker { get; set; } // Пути до папок относительно ExerciseDirName. Перетирают файлы из exerciseDirName

		[XmlElement("excludePathForStudent")]
		public string[] PathsToExcludeForStudent { get; set; } // Шаблоны путей до файлов внутри ExerciseDirName. Нужно вручную учитывать папку src, если путь от корня
		
		[XmlElement("checkInitialSolution")]
		public bool CheckInitialSolution { get; set; } = true;
		
		[XmlAttribute("noStudentZip")] // Не отдавать zip студенту
		public bool NoStudentZip { get; set; }
		
		[XmlElement("dockerImageName")] // см. DockerImageNameRegex
		public string DockerImageName { get; set; }

		[XmlIgnore]
		public static readonly Regex DockerImageNameRegex = new Regex("^[-_a-z.]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		
		[XmlElement("run")]
		public string RunCommand { get; set; } // см. RunCommandRegex
		
		[XmlIgnore]
		public static readonly Regex RunCommandRegex = new Regex("^[-_a-z. ;|>]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		
		[XmlIgnore]
		public DirectoryInfo SlideDirectoryPath { get; set; }
		
		[XmlIgnore]
		public DirectoryInfo ExerciseDirectory => new DirectoryInfo(Path.Combine(SlideDirectoryPath.FullName, ExerciseDirName));

		[XmlIgnore]
		public DirectoryInfo CourseDirectory { get; set; }
		
		[XmlIgnore]
		public FileInfo UserCodeFile => ExerciseDirectory.GetFile(UserCodeFilePath);
		
		[XmlIgnore]
		public string InitialUserCodeFilePath
		{
			get
			{
				var parts = UserCodeFilePath.Split('.');
				return string.Join(".", parts.Take(parts.Length - 1).Concat(new[] { "Initial", parts.Last() }));
			}
		}

		[XmlIgnore]
		public FileInfo InitialUserCodeFile => ExerciseDirectory.GetFile(InitialUserCodeFilePath);

		[XmlIgnore]
		public FileInfo CorrectSolutionFile => UserCodeFile;
		
		[XmlIgnore]
		public string CorrectSolutionFilePath => UserCodeFilePath;

		[XmlIgnore]
		private static readonly string[] wrongAnswerPatterns = { "*.WrongAnswer.*", "*.WA.*" };
		
		[XmlIgnore]
		private static readonly string[] initialPatterns = { "*.Initial.*" };
		
		[XmlIgnore]
		public List<FileInfo> WrongAnswerFiles => 
			wrongAnswerPatterns.SelectMany(p => ExerciseDirectory.GetFiles(p, SearchOption.AllDirectories))
				.ToList();
		
		[XmlIgnore]
		private List<FileInfo> InitialFiles => 
			initialPatterns.SelectMany(p => ExerciseDirectory.GetFiles(p, SearchOption.AllDirectories))
				.ToList();

		private string InitialFilePathToNotInitial(string path)
		{
			var notInitial = path;
			initialPatterns.ForEach(p => notInitial = notInitial.Replace(p.Replace(".*", "").Replace("*", ""), ""));
			return notInitial;
		}
		
		[XmlIgnore]
		public Lazy<string> SolutionRegionContent;

		[XmlIgnore]
		public Lazy<string> InitialRegionContent;

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
			ExpectedOutput = ExpectedOutput ?? "";
			SolutionRegionContent = new Lazy<string>(() => GetRegionContent(UserCodeFile));
			InitialRegionContent = new Lazy<string>(() => GetRegionContent(InitialUserCodeFile));
			ExerciseInitialCode = NoStudentZip
				? GetNoStudentZipInitialCode()
				: ExerciseInitialCode ?? "// Вставьте сюда финальное содержимое файла " + UserCodeFilePath;
			CheckForPlagiarism = false;
			yield return this;

			var correctSolution = GetCorrectSolution();
			if (correctSolution != null)
			{
				yield return new MarkdownBlock("### Решение") { Hide = true };
				yield return new CodeBlock(GetCorrectSolution(), Language) { Hide = true };
			}
		}

		public string GetCorrectSolution()
		{
			return SolutionRegionContent.Value ?? (CorrectSolutionFile.Exists ? CorrectSolutionFile.ContentAsUtf8() : null);
		}
		
		public string GetInitialCode()
		{
			return NoStudentZip ? GetNoStudentZipInitialCode() : InitialUserCodeFile.ContentAsUtf8();
		}

		private string GetNoStudentZipInitialCode()
		{
			return ExerciseInitialCode
				= InitialRegionContent.Value
					?? (InitialUserCodeFile.Exists ? InitialUserCodeFile.ContentAsUtf8() : null)
					?? ExerciseInitialCode;
		}
		

		public string GetRegionContent(FileInfo file)
		{
			return Region == null || !file.Exists
				? null
				: new CommonSingleRegionExtractor(file.ContentAsUtf8()).GetRegion(new Label { Name = Region });
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
				.Concat(initialPatterns)
				.Concat(wrongAnswerPatterns)
				.ToList();

			var toUpdateDirectories = PathsToIncludeForChecker
				.Select(pathToInclude => new DirectoryInfo(Path.Combine(ExerciseDirectory.FullName, pathToInclude)));

			var toUpdate = GetCodeFile(code).ToList();
			var zipBytes = ToZip(ExerciseDirectory, excluded, toUpdate, toUpdateDirectories);
			log.Info($"Собираю zip-архив для проверки: zip-архив собран, {zipBytes.Length} байтов");
			return zipBytes;
		}
		
		private IEnumerable<FileContent> GetCodeFile(string code)
		{
			if (Region == null)
				yield return new FileContent { Path = UserCodeFilePath, Data = Encoding.UTF8.GetBytes(code) };
			else
			{
				var fullCode = new CommonSingleRegionExtractor((InitialUserCodeFile.Exists ? InitialUserCodeFile : UserCodeFile).ContentAsUtf8())
					.ReplaceRegionContent(new Label{Name = Region}, code);
				yield return new FileContent { Path = UserCodeFilePath, Data = Encoding.UTF8.GetBytes(fullCode) };
			}
		}
		
		public byte[] GetZipBytesForStudent()
		{
			var excluded = (PathsToExcludeForStudent ?? new string[0])
				.Concat(initialPatterns)
				.Concat(wrongAnswerPatterns)
				.Concat(new[] { "checking/*" })
				.ToList();
			
			var toUpdate = ReplaceWithInitialFiles().ToList();
			
			var zipBytes = ToZip(ExerciseDirectory, excluded, toUpdate);

			log.Info($"Собираю zip-архив для студента: zip-архив собран, {zipBytes.Length} байтов");
			return zipBytes;
		}

		private IEnumerable<FileContent> ReplaceWithInitialFiles()
		{
			foreach (var initialFile in InitialFiles)
			{
				var relativeFilePath = initialFile.GetRelativePath(ExerciseDirectory.FullName);
				var notInitial = InitialFilePathToNotInitial(relativeFilePath);
				yield return new FileContent { Path = notInitial, Data = initialFile.ReadAllContent() };
			}
		}
	}
}