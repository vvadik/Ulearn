using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Build.Evaluation;
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

		public static void PrepareForChecking(Project proj, ProjectExerciseBlock exerciseBlock)
		{
			PrepareForChecking(proj, exerciseBlock.StartupObject);
		}

		public static void PrepareForChecking(Project proj, string startupObject)
		{
			proj.SetProperty("StartupObject", startupObject);
			proj.SetProperty("OutputType", "Exe");
			proj.SetProperty("UseVSHostingProcess", "false");
			ResolveLinks(proj);
		}

		private static void ResolveLinks(Project project)
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