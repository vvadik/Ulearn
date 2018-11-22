using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ulearn.Core.Helpers;

namespace Ulearn.Core.Extensions
{
	public static class FileExtensions
	{
		public static IEnumerable<FileInfo> GetFilesByMask(this DirectoryInfo directory, string mask)
		{
			if (mask.Contains(".."))
				throw new ArgumentException($"{nameof(mask)} can not contain \"..\"", nameof(mask));
			
			return GlobSearcher.Glob(Path.Combine(directory.FullName, mask)).Select(path => new FileInfo(path));
		}
	}
}