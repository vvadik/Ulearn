using System;
using System.IO;
using System.Linq;
using uLearn.Extensions;

namespace uLearn.CourseTool
{
	public static class EdxInteraction
	{
		public static string GetTarGzFile(string dir)
		{
			var archives = dir.GetFiles("*.tar.gz");
			if (archives.Length == 1)
				return archives[0];
			throw new Exception($"Can't decide which archive to extract! Remove all other tar.gz files. Found: {string.Join(", ", archives)}");
		}

		public static void ExtractEdxCourseArchive(string baseDir, string tarGzFilepath)
		{
			var filename = tarGzFilepath;
			Console.WriteLine($"Extracting {filename}");

			var extractedTarDirectory = Path.Combine(baseDir, ".extracted-" + Path.GetFileName(filename).EnsureNotNull().Replace(".tar.gz", ""));
			ArchiveManager.ExtractTar(filename, extractedTarDirectory);
			var extractedOlxDirectory = Directory.GetDirectories(extractedTarDirectory).Single();
			Utils.DeleteDirectoryIfExists(baseDir + "/olx");
			Directory.Move(extractedOlxDirectory, baseDir + "/olx");
			Directory.Delete(extractedTarDirectory);
		}

		public static void CreateEdxCourseArchive(string baseDir, string courseName)
		{
			Environment.CurrentDirectory = baseDir;
			var outputTarFilename = $"{courseName}.tar.gz";
			Console.WriteLine($"Creating archive {outputTarFilename}");
			Utils.DeleteFileIfExists(outputTarFilename);
			ArchiveManager.CreateTar(outputTarFilename, "olx");
		}
	}
}