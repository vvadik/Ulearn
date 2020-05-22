using System;
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

			using (var zip = new ZipFile())
			{
				foreach (var update in courseUpdates)
				{
					if (Directory.Exists(update.FullPath))
					{
						zip.AddDirectory(update.FullPath, update.RelativePath);
					}

					if (File.Exists(update.FullPath))
					{
						zip.AddFile(update.FullPath, Path.GetDirectoryName(update.RelativePath));
					}
				}

				zip.AddEntry("deleted.txt", deletedFileContent);
				
				zip.Save(ms);
			}
			#region Create arhive in DEBUG
#if DEBUG
			using (var zip1 = new ZipFile())
			{
				foreach (var update in courseUpdates)
				{
					if (Directory.Exists(update.FullPath))
					{
						zip1.AddDirectory(update.FullPath, update.RelativePath);
					}

					if (File.Exists(update.FullPath))
					{
						zip1.AddFile(update.FullPath, Path.GetDirectoryName(update.RelativePath));
					}
				}
				var guid = Guid.NewGuid();
				zip1.AddEntry("deleted.txt", deletedFileContent);
				zip1.Save($"../{guid.ToString()}.zip");
				Console.WriteLine($"DEBUG is on ../{guid.ToString()}.zip was created");
			}
#endif
			#endregion			
			return ms;
		}

		public static MemoryStream CreateZipByFolder(string pathToFolder)
		{
			using var zip = new ZipFile(Encoding.UTF8);
			zip.AddDirectory(pathToFolder);
			var ms = new MemoryStream();
			zip.Save(ms);

#if DEBUG
			using var zip1 = new ZipFile();
			zip1.AddDirectory(pathToFolder);
			zip1.Save("../temp.zip");
			Console.WriteLine("DEBUG is on ../temp.zip was created");
#endif
			return ms;
		}
	}
}