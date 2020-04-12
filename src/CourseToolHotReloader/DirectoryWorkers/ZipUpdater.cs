using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CourseToolHotReloader.Dtos;
using Ionic.Zip;

namespace CourseToolHotReloader.DirectoryWorkers
{
	public class ZipUpdater
	{
		public static Guid CreateZipByUpdates(IList<ICourseUpdate> courseUpdates, IList<ICourseUpdate> deletedFiles)
		{
			var deletedFileContent = string.Join("\r\n", deletedFiles.Select(u => u.RelativePath));
			var guid = Guid.NewGuid();

			using (var zip = new ZipFile())
			{
				foreach (var update in courseUpdates)
				{
					zip.AddFile(update.FullPath, Path.GetDirectoryName(update.RelativePath));
				}

				zip.AddEntry("deleted.txt", deletedFileContent);

				zip.Save($"{guid.ToString()}.zip");
			}

			return guid;
		}
	}
}