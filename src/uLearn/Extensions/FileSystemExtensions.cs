using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using SearchOption = Microsoft.VisualBasic.FileIO.SearchOption;

namespace uLearn.Extensions
{
	public static class FileSystemExtensions
	{
		public static bool HasSubdirectory(this DirectoryInfo root, string name)
		{
			return root.Subdirectory(name).Exists;
		}

		public static DirectoryInfo Subdirectory(this DirectoryInfo root, string name)
		{
			return new DirectoryInfo(Path.Combine(root.FullName, name));
		}

		public static FileInfo GetFile(this DirectoryInfo dir, string filename)
		{
			return new FileInfo(Path.Combine(dir.FullName, filename));
		}

		public static DirectoryInfo GetSubdir(this DirectoryInfo dir, string name)
		{
			return new DirectoryInfo(Path.Combine(dir.FullName, name));
		}

		public static DirectoryInfo GetOrCreateSubdir(this DirectoryInfo dir, string name)
		{
			var subdir = dir.GetSubdir(name);
			if (!subdir.Exists)
				subdir.Create();
			return subdir;
		}

		public static string GetRelativePath(this FileSystemInfo file, string folder)
		{
			var pathUri = new Uri(file.FullName);
			if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
				folder += Path.DirectorySeparatorChar;
			var folderUri = new Uri(folder);
			return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
		}

		public static IEnumerable<string> GetRelativePathsOfFiles(this DirectoryInfo dir)
		{
			return FileSystem.GetFiles(dir.FullName, SearchOption.SearchAllSubDirectories)
				.Select(path => new FileInfo(path))
				.Select(fi => fi.GetRelativePath(dir.FullName));
		}

		public static string[] GetFilenames(this DirectoryInfo di, string dirPath)
		{
			var dir = di.GetSubdir(dirPath);
			if (!dir.Exists)
				throw new Exception("No " + dirPath + " in " + di.Name);
			return dir.GetFiles().Select(f => Path.Combine(dirPath, f.Name)).ToArray();
		}
	}
}