using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Evaluation;
using uLearn.Extensions;
using uLearn.Model.Blocks;

namespace uLearn
{
	public class FileToCopy
	{
		public FileToCopy(string sourceFile, string destinationFile)
		{
			SourceFile = sourceFile;
			DestinationFile = destinationFile;
		}

		public readonly string SourceFile;
		public readonly string DestinationFile;
	}

	public static class ProjModifier
	{
		internal static void PrepareForStudentZip(Project proj, ProjectExerciseBlock ex)
		{
			var toExclude = FindItemNames(proj, ex.NeedExcludeFromStudentZip).ToList();
			var solutionsOfOtherTasks = toExclude.Where(n => ProjectExerciseBlock.IsAnySolution(n) && ex.CorrectSolutionPath != n).ToList();

			var userCodeFilepathsOfOtherTasks = solutionsOfOtherTasks.Select(ProjectExerciseBlock.SolutionFilepathToUserCodeFilepath);

			RemoveCheckingFromCsproj(proj);
			SetFilepathItemTypeToCompile(proj, userCodeFilepathsOfOtherTasks.Concat(new[] { ex.UserCodeFilePath }));
			ResolveLinks(proj);
			ExcludePaths(proj, toExclude);
		}

		public static void RemoveCheckingFromCsproj(Project proj)
		{
			var toRemove = proj.Items.Where(IsChecking).ToList();
			proj.RemoveItems(toRemove);
		}

		private static bool IsChecking(ProjectItem item)
		{
			return
				item.EvaluatedInclude.StartsWith("checking" + Path.DirectorySeparatorChar)
				|| item.DirectMetadata.Any(md => md.Name == "Link" && md.EvaluatedValue.StartsWith("checking" + Path.DirectorySeparatorChar));
		}

		public static void PrepareForCheckingUserCode(Project proj, ProjectExerciseBlock ex, List<string> excludedPaths)
		{
			var solutionRelativePath = ex.ExerciseFolder.GetRelativePathsOfFiles()
				.SingleOrDefault(n => n.Equals(ex.CorrectSolutionPath, StringComparison.InvariantCultureIgnoreCase));

			if (solutionRelativePath != null)
				excludedPaths.Add(solutionRelativePath);

			SetFilepathItemTypeToCompile(proj, ex.UserCodeFilePath);
			PrepareForChecking(proj, ex.StartupObject, excludedPaths);
		}

		public static void SetFilepathItemTypeToCompile(Project proj, IEnumerable<string> files)
		{
			foreach (var f in files)
				SetFilepathItemType(proj, f, "Compile");
		}

		public static void SetFilepathItemTypeToCompile(Project proj, string fileName) => SetFilepathItemType(proj, fileName, "Compile");

		public static void SetFilepathItemType(Project proj, string fileName, string type)
		{
			proj.Items.Single(i => i.UnevaluatedInclude.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).ItemType = type;
		}

		private static IEnumerable<string> FindItemNames(Project proj, Func<string, bool> predicate)
		{
			return proj.Items.Select(i => i.UnevaluatedInclude).Where(predicate);
		}

		public static void PrepareForChecking(Project proj, string startupObject, IReadOnlyList<string> excludedPaths)
		{
			proj.SetProperty("StartupObject", startupObject);
			proj.SetProperty("OutputType", "Exe");
			proj.SetProperty("UseVSHostingProcess", "false");
			ResolveLinks(proj);
			ExcludePaths(proj, excludedPaths);
		}

		private static void ExcludePaths(Project proj, IReadOnlyList<string> excludedPaths)
		{
			var toRemove = proj.Items.Where(item => excludedPaths.Contains(item.UnevaluatedInclude, StringComparer.InvariantCultureIgnoreCase)).ToList();
			proj.RemoveItems(toRemove);
		}

		public static void ResolveLinks(Project project)
		{
			var files = ReplaceLinksWithItemsCopiedToProjectDir(project);
			foreach (var file in files)
			{
				var dst = Path.Combine(project.DirectoryPath, file.DestinationFile);
				var src = Path.Combine(project.DirectoryPath, file.SourceFile);
				var dstDir = Path.GetDirectoryName(dst).EnsureNotNull();
				Directory.CreateDirectory(dstDir);
				File.Copy(src, dst, true);
			}
		}

		public static List<FileToCopy> ReplaceLinksWithItemsCopiedToProjectDir(Project project)
		{
			var linkedItems = (from item in project.Items
				let meta = item.DirectMetadata.FirstOrDefault(md => md.Name == "Link")
				where meta != null
				select new { item, newPath = ChangeNameToGitIgnored(meta.EvaluatedValue) }).ToList();
			var copies = new List<FileToCopy>();
			foreach (var link in linkedItems)
			{
				copies.Add(new FileToCopy(link.item.EvaluatedInclude, link.newPath));
				link.item.UnevaluatedInclude = link.newPath;
				link.item.RemoveMetadata("Link");
			}
			return copies;
		}

		private static string ChangeNameToGitIgnored(string filename)
		{
			var d = Path.GetDirectoryName(filename) ?? "";
			var fn = Path.GetFileName(filename);
			return Path.Combine(d, "~$" + fn);
		}

		public static byte[] ModifyCsproj(FileInfo csproj, Action<Project> changingAction)
		{
			var proj = new Project(csproj.FullName, null, null, new ProjectCollection());
			return ModifyCsproj(changingAction, proj);
		}

		private static byte[] ModifyCsproj(Action<Project> changingAction, Project proj)
		{
			changingAction?.Invoke(proj);
			using (var memoryStream = new MemoryStream())
			using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
			{
				proj.Save(streamWriter);
				return memoryStream.ToArray();
			}
		}
	}
}