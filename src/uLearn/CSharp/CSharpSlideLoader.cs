using System.IO;
using System.Linq;

namespace uLearn.CSharp
{
	public class CSharpSlideLoader : ISlideLoader
	{
		public string Extension { get { return ".cs"; } }

		public Slide Load(FileInfo file, string unitName, int slideIndex, CourseSettings settings)
		{
			var sourceCode = file.ContentAsUtf8();
			var prelude = GetPrelude(file.Directory);
			var di = file.Directory;
			return SlideParser.ParseCode(sourceCode, new SlideInfo(unitName, file, slideIndex), prelude, di);
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