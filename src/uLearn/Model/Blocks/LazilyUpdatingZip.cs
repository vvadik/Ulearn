using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Ionic.Zip;
using Ionic.Zlib;
using uLearn.Extensions;

namespace uLearn.Model.Blocks
{
	public class LazilyUpdatingZip
	{
		private readonly DirectoryInfo dir;
		private readonly string[] excludedDirs;
	    private readonly string excludedFilesNamePattern;
		private readonly Func<FileInfo, byte[]> getFileContent;
		private readonly FileInfo zipFile;

	    public LazilyUpdatingZip(
			DirectoryInfo dir, 
			string[] excludedDirs, 
			string excludedFilesNamePattern, 
			Func<FileInfo, byte[]> getFileContent, 
			FileInfo zipFile)
		{
			this.dir = dir;
			this.excludedDirs = excludedDirs;
			this.excludedFilesNamePattern = excludedFilesNamePattern;
			this.getFileContent = getFileContent;
			this.zipFile = zipFile;
		}

		public void UpdateZip()
		{
			if (IsActual())
				return;
			using (var zip = new ZipFile())
			{
				zip.CompressionLevel = CompressionLevel.BestSpeed;
				foreach (var f in EnumerateFiles())
				{
					var newContent = getFileContent(f);
					if (newContent == null)
						zip.AddFile(f.FullName, Path.GetDirectoryName(f.GetRelativePath(dir.FullName)));
					else
						zip.AddEntry(f.GetRelativePath(dir.FullName), newContent);

				}
				zip.Save(zipFile.FullName);
			}
		}

		private bool IsActual()
		{
			var lastWriteTime = EnumerateFiles().Max(f => f.LastWriteTimeUtc);
			return zipFile.Exists && zipFile.LastWriteTimeUtc > lastWriteTime;
		}

		public IEnumerable<FileInfo> EnumerateFiles()
		{
			return EnumerateFiles(dir);
		}

		private IEnumerable<FileInfo> EnumerateFiles(DirectoryInfo aDir)
		{
			return aDir.GetAllFiles().Where(f =>
				!(excludedDirs.Contains(f.Directory?.Name) || Regex.IsMatch(f.Name, excludedFilesNamePattern)));
		}
	}
}