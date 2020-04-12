using System;
using System.Collections.Generic;
using System.IO;
using CourseToolHotReloader.Dtos;
using Ionic.Zip;

namespace CourseToolHotReloader.DirectoryWorkers
{
	public class ZipHelper
	{
		public static Guid CreateNewZipByUpdates(IList<ICourseUpdate> courseUpdates)
		{
			var guid = Guid.NewGuid();

			using (var zip = new ZipFile())
			{
				foreach (var update in courseUpdates)
				{
					zip.AddFile(update.FullPath, Path.GetDirectoryName(update.RelativePath));
				}

				zip.Save($"{guid.ToString()}.zip");
			}

			return guid;
		}
	}
}