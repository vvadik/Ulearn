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
			return SlideParser.ParseCode(sourceCode, new SlideInfo(unitName, file, slideIndex), prelude, fn => GetInclude(file.Directory, fn));
		}

		private string GetInclude(DirectoryInfo directory, string filename)
		{
			var fileInfo = directory.GetFile(filename);
			if (fileInfo.Exists) return fileInfo.ContentAsUtf8();
			throw new Exception("Include " + filename + " not found in " + directory.Name);
		}

		private static string GetPrelude(DirectoryInfo dir)
		{
			var preludeFile = new[] { dir, dir.Parent }.
				SelectMany(d => d.GetFiles("prelude.*", SearchOption.TopDirectoryOnly))
				.FirstOrDefault(f => f.Exists);
			return preludeFile == null ? "" : preludeFile.ContentAsUtf8();
		}
	}
}