using System;
using System.IO;
using System.Linq;

namespace uLearn.CSharp
{
	public class CSharpSlideLoader : ISlideLoader
	{
		public string Extension { get { return ".cs"; } }

		public Slide Load(FileInfo file, string unitName, int slideIndex)
		{
			var sourceCode = file.ContentAsUtf8();
			var prelude = GetPrelude(file.Directory);
			var fs = new FileSystem(file.Directory);
			return SlideParser.ParseCode(sourceCode, new SlideInfo(unitName, file, slideIndex), prelude, fs);
		}

		private static string GetPrelude(DirectoryInfo dir)
		{
			var preludeFile = new[] { dir, dir.Parent }.
				SelectMany(d => d.GetFiles("prelude.*", SearchOption.TopDirectoryOnly))
				.FirstOrDefault(f => f.Exists);
			return preludeFile == null ? "" : preludeFile.ContentAsUtf8();
		}
	}

	
	public interface IFileSystem
	{
		string GetContent(string filepath);
		string[] GetFilenames(string dirPath);
	}

	public class FileSystem : IFileSystem
	{
		private readonly DirectoryInfo baseDir;

		public FileSystem(DirectoryInfo baseDir)
		{
			this.baseDir = baseDir;
		}

		public string GetContent(string filepath)
		{
			var fileInfo = baseDir.GetFile(filepath);
			if (fileInfo.Exists) return fileInfo.ContentAsUtf8();
				throw new Exception("No " + filepath + " in " + baseDir.Name);
		}

		public string[] GetFilenames(string dirPath)
		{
			var dir = new DirectoryInfo(Path.Combine(baseDir.FullName, dirPath));
			if (!dir.Exists)
				throw new Exception("No " + dirPath + " in " + baseDir.Name);
			return dir.GetFiles().Select(f => dirPath + "/" + f.Name).ToArray();

		}
	}
}