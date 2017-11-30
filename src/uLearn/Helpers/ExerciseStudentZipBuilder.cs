using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using uLearn.Extensions;
using uLearn.Model.Blocks;

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
				file => ReplaceCsproj(block, file), zipFile);
			ResolveCsprojLinks(block);
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
		
		private static byte[] ReplaceCsproj(ProjectExerciseBlock block, FileInfo file)
		{
			if (!file.Name.Equals(block.CsprojFileName, StringComparison.InvariantCultureIgnoreCase))
				return null;
			return ProjModifier.ModifyCsproj(file, proj => ProjModifier.PrepareForStudentZip(proj, block));
		}
		
		private static void ResolveCsprojLinks(ProjectExerciseBlock block)
		{
			ProjModifier.ModifyCsproj(block.ExerciseFolder.GetFile(block.CsprojFileName), ProjModifier.ResolveLinks);
		}
	}
}