using System;
using System.IO;
using System.Linq;

namespace uLearn.CSharp
{

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