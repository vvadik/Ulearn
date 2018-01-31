using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Evaluation;
using uLearn.Extensions;
using uLearn.Model.Blocks;
using Ulearn.Common;
using Ulearn.Common.Extensions;

namespace uLearn.Helpers
{
	public class ExerciseStudentZipBuilder
	{
		private static readonly Regex anySolutionNameRegex = new Regex("(.+)\\.Solution\\.cs", RegexOptions.IgnoreCase);
		private static readonly Regex anyWrongAnswerNameRegex = new Regex("(.+)\\.WrongAnswer\\.(.+)\\.cs", RegexOptions.IgnoreCase);

		public static bool IsAnyWrongAnswerOrAnySolution(string name) => anyWrongAnswerNameRegex.IsMatch(name) || anySolutionNameRegex.IsMatch(name);
		public static bool IsAnySolution(string name) => anySolutionNameRegex.IsMatch(name);		
		
		public void BuildStudentZip(Slide slide, FileInfo zipFile)
		{
			var block = (slide as ExerciseSlide)?.Exercise as ProjectExerciseBlock;
			if (block == null)
				throw new InvalidOperationException($"Can't generate student zip for non-project exercise block: slide \"{slide.Title}\" ({slide.Id})");
			
			var zip = new LazilyUpdatingZip(
				block.ExerciseFolder,
				new[] { "checking", "bin", "obj" },
				file => NeedExcludeFromStudentZip(block, file),
				file => GetFileContentInStudentZip(block, file),
				ResolveCsprojLinks(block),
				zipFile);
			zip.UpdateZip();
		}
		
		private static bool NeedExcludeFromStudentZip(ProjectExerciseBlock block, FileInfo file)
		{
			var relativeFilePath = file.GetRelativePath(block.ExerciseFolder.FullName);
			return NeedExcludeFromStudentZip(block, relativeFilePath);
		}

		public static bool NeedExcludeFromStudentZip(ProjectExerciseBlock block, string filepath)
		{
			return IsAnyWrongAnswerOrAnySolution(filepath) ||
					block.PathsToExcludeForStudent != null && block.PathsToExcludeForStudent.Any(p => p == filepath);
		}
		
		private static byte[] GetFileContentInStudentZip(ProjectExerciseBlock block, FileInfo file)
		{
			if (!file.Name.Equals(block.CsprojFileName, StringComparison.InvariantCultureIgnoreCase))
				return null;
			return ProjModifier.ModifyCsproj(file, proj => ProjModifier.PrepareForStudentZip(proj, block));
		}
		
		public static IEnumerable<FileContent> ResolveCsprojLinks(ProjectExerciseBlock block)
		{
			return ResolveCsprojLinks(block.CsprojFile, block.BuildEnvironmentOptions.ToolsVersion);
		}
		
		public static IEnumerable<FileContent> ResolveCsprojLinks(FileInfo csprojFile, string toolsVersion)
		{
			var project = new Project(csprojFile.FullName, null, toolsVersion, new ProjectCollection());
			var filesToCopy = ProjModifier.ReplaceLinksWithItemsAndReturnWhatToCopy(project);
			foreach (var fileToCopy in filesToCopy)
			{
				var fullSourcePath = Path.Combine(project.DirectoryPath, fileToCopy.SourceFile);
				yield return new FileContent { Path = fileToCopy.DestinationFile, Data = File.ReadAllBytes(fullSourcePath) };
			}
		}
	}
}