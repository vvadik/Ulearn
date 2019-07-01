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
		
		[XmlElement("studentZipIsCompilable")]
		public bool StudentZipIsCompilable { get; set; } = true;
		
		[XmlAttribute("noStudentZip")] // Не отдавать zip студенту
		public bool NoStudentZip { get; set; }
		
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
		
		public string InitialUserCodeFilePath
		{
			get
			{
				var parts = UserCodeFilePath.Split('.');
				return string.Join(".", parts.Take(parts.Length - 1).Concat(new[] { "Initial", parts.Last() }));
			}
		}

		public FileInfo InitialUserCodeFile => ExerciseDirectory.GetFile(InitialUserCodeFilePath);
		public DirectoryInfo UserCodeFileParentDirectory => UserCodeFile.Directory;
		
		public FileInfo CorrectSolutionFile => UserCodeFile;
		
		public string UserCodeFileNameWithoutExt => Path.GetFileNameWithoutExtension(UserCodeFilePath);
		
		private Regex WrongAnswersAndSolutionNameRegex => new Regex(new Regex("^") + UserCodeFileNameWithoutExt + new Regex("\\.(WrongAnswer|WA)\\..+"));

		public bool IsWrongAnswer(string name) => WrongAnswersAndSolutionNameRegex.IsMatch(name);

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
			SolutionRegionContent = new Lazy<string>(() => Region == null ? null : new CommonSingleRegionExtractor(UserCodeFile.ContentAsUtf8()).GetRegion(new Label{Name = Region}));
			InitialRegionContent = new Lazy<string>(() =>  Region == null ? null : new CommonSingleRegionExtractor(InitialUserCodeFile.ContentAsUtf8()).GetRegion(new Label{Name = Region}));
			Validate();
			ExpectedOutput = ExpectedOutput ?? "";
			if (NoStudentZip)
			{
				ExerciseInitialCode
					= InitialRegionContent.Value
						?? (InitialUserCodeFile.Exists ? InitialUserCodeFile.ContentAsUtf8() : null)
						?? ExerciseInitialCode;
			}
			else
			{
				ExerciseInitialCode = ExerciseInitialCode ?? "// Вставьте сюда финальное содержимое файла " + UserCodeFilePath;
			}
			yield return this;
			
			if (CorrectSolutionFile.Exists)
			{
				yield return new MarkdownBlock("### Решение") { Hide = true };
				yield return new CodeBlock(SolutionRegionContent.Value ?? CorrectSolutionFile.ContentAsUtf8(), Language) { Hide = true };
			}
		}

		private void Validate()
		{
			if (DockerImageName == null)
				throw new ArgumentException("dockerImageName not specified");
			if (DockerImageName == null)
				throw new ArgumentException("run not specified");
			if (!DockerImageNameRegex.Match(DockerImageName).Success)
				throw new ArgumentException($"dockerImageName {DockerImageName}");
			if (!RunCommandRegex.Match(RunCommand).Success)
				throw new ArgumentException($"run {RunCommand}");
			if (!ExerciseDirectory.Exists)
				throw new ArgumentException($"exerciseDirName '{ExerciseDirName}' not exists");
			if (!UserCodeFile.Exists)
				throw new ArgumentException($"userCodeFile '{UserCodeFilePath}' not exists");
			if (Region != null && !NoStudentZip)
				throw new ArgumentException($"Region works only if StudentZipIsDisabled true");
			if (!InitialUserCodeFile.Exists && (!NoStudentZip || NoStudentZip && ExerciseInitialCode == null))
				throw new ArgumentException($"'{InitialUserCodeFilePath}' file not exists");
			foreach (var pathToIncludeForChecker in PathsToIncludeForChecker)
			{
				var di = new DirectoryInfo(Path.Combine(ExerciseDirectory.FullName, pathToIncludeForChecker));
				if (!CourseDirectory.IsInside(di))
					throw new ArgumentException($"includePathForChecker '{pathToIncludeForChecker}' is not in subtree of directory with course.xml");
				if (!di.Exists)
					throw new ArgumentException($"includePathForChecker '{pathToIncludeForChecker}' not exists");
			}
			if (Region != null)
			{
				if(SolutionRegionContent.Value == null)
					throw new ArgumentException($"Region '{Region}' not exists in file '{UserCodeFilePath}'");
				if(ExerciseInitialCode == null && InitialRegionContent.Value == null)
					throw new ArgumentException($"Region '{Region}' not exists in file '{InitialUserCodeFilePath}'");
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
			var zipBytes = ExerciseDirectory.ToZip(excluded, toUpdate, toUpdateDirectories);
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