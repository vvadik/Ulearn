using System.IO;

namespace uLearn
{
	public class SlideInfo
	{
		public int Index { get; set; }
		public string UnitName { get; private set; }
		public FileInfo SlideFile { get; set; }

		public SlideInfo(string unitName, FileInfo slideFile, int index)
		{
			Index = index;
			UnitName = unitName;
			SlideFile = slideFile;
		}

		public string DirectoryRelativePath
		{
			get
			{
				// ReSharper disable PossibleNullReferenceException
				var courseDir = SlideFile.Directory.Parent.Name;
				var unitDir = SlideFile.Directory.Name;
				// ReSharper restore PossibleNullReferenceException
				return courseDir + "/" + unitDir;
			}
		}
	}
}
