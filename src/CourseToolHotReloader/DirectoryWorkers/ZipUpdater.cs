using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CourseToolHotReloader.Dtos;
using Ionic.Zip;

namespace CourseToolHotReloader.DirectoryWorkers
{
	public class ZipUpdater
	{
		public static MemoryStream CreateZipByUpdates(IList<ICourseUpdate> courseUpdates, IList<ICourseUpdate> deletedFiles)
		{
			var deletedFileContent = string.Join("\r\n", deletedFiles.Select(u => u.RelativePath));
			var ms = new MemoryStream();

			using (var zip = new ZipFile(Encoding.UTF8))
			{
				foreach (var update in courseUpdates)
				{
					if (Directory.Exists(update.FullPath))
						zip.AddDirectory(update.FullPath, update.RelativePath);

					if (File.Exists(update.FullPath))
						zip.AddFile(update.FullPath, Path.GetDirectoryName(update.RelativePath));
				}

				zip.AddEntry("deleted.txt", deletedFileContent);

				zip.Save(ms);
			}

			return ms;
		}

		public static MemoryStream CreateZipByFolder(string pathToFolder)
		{
			using var zip = new ZipFile(Encoding.UTF8);
			zip.AddDirectory(pathToFolder);
			var ms = new MemoryStream();
			zip.Save(ms);

			return ms;
		}
	}
}