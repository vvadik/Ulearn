using System.Collections.Generic;
using System.IO.Compression;

namespace CourseToolHotReloader.DirectoryWorkers
{
	public class ZipHelper
	{
		public static void PackZip(List<string> files)
		{
			ZipFile.CreateFromDirectory("hello", "Backup.zip");
		}
	}
}