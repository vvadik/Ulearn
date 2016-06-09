using System;
using System.IO;

namespace uLearn
{
	public class Utils
	{
		public static string GetSource(string courseId, Guid slideId, CourseManager courseManager, string code)
		{
			return courseId == "web" && slideId == Guid.Empty
				? code
				: ((ExerciseSlide)courseManager.GetCourse(courseId).GetSlideById(slideId))
					.Exercise
					.Solution
					.BuildSolution(code)
					.SourceCode;
		}

		public static string NewNormalizedGuid()
		{
			return Guid.NewGuid().ToString("D");
		}

		public static string GetNormalizedGuid(string guid)
		{
			return Guid.Parse(guid).ToString("D");
		}

		public static string GetNormalizedGuid(Guid guid)
		{
			return guid.ToString("D");
		}

		public static string GetRootDirectory()
		{
			return AppDomain.CurrentDomain.BaseDirectory;
		}

		public static void DeleteFileIfExists(string file)
		{
			if (File.Exists(file))
				File.Delete(file);
		}

		public static void DeleteDirectoryIfExists(string directory)
		{
			if (Directory.Exists(directory))
				Directory.Delete(directory, true);
		}

		public static string GetPass()
		{
			var password = "";
			ConsoleKeyInfo info;
			while ((info = Console.ReadKey(true)).Key != ConsoleKey.Enter)
				if (info.Key != ConsoleKey.Backspace)
					password += info.KeyChar;
				else if (!string.IsNullOrEmpty(password))
					password = password.Substring(0, password.Length - 1);
			return password;
		}

		public static void DirectoryCopy(string source, string dest, bool recursive)
		{
			var dir = new DirectoryInfo(source);
			var dirs = dir.GetDirectories();

			if (!dir.Exists)
				throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + source);

			if (!Directory.Exists(dest))
				Directory.CreateDirectory(dest);

			foreach (var file in dir.GetFiles())
			{
				var tempPath = Path.Combine(dest, file.Name);
				file.CopyTo(tempPath, true);
			}

			if (recursive)
			{
				foreach (var subDir in dirs)
				{
					var tempPath = Path.Combine(dest, subDir.Name);
					DirectoryCopy(subDir.FullName, tempPath, true);
				}
			}
		}
	}
}
