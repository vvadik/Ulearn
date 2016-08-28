using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Ionic.Zlib;

namespace uLearn.Model.Blocks
{
	public class LazilyUpdatingZip
	{
		private readonly DirectoryInfo dir;
		private readonly string[] excludedDirs;
		private readonly Func<FileInfo, byte[]> getFileContent;
		private readonly FileInfo zipFile;

		public LazilyUpdatingZip(DirectoryInfo dir, string[] excludedDirs, Func<FileInfo, byte[]> getFileContent, FileInfo zipFile)
		{
			this.dir = dir;
			this.excludedDirs = excludedDirs;
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

		private IEnumerable<FileInfo> EnumerateFiles()
		{
			return EnumerateFiles(dir);
		}

		private IEnumerable<FileInfo> EnumerateFiles(DirectoryInfo aDir)
		{
			foreach (var f in aDir.GetFiles())
				yield return f;
			var dirs = aDir.GetDirectories().Where(d => !excludedDirs.Contains(d.Name));
			foreach (var subdir in dirs)
				foreach (var f in EnumerateFiles(subdir))
					yield return f;
		}
	}
}