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
				return CourseUnitUtls.GetDirectoryRelativeWebPath(SlideFile);
			}
		}
	}

	public class CourseUnitUtls
	{
		public static string GetDirectoryRelativeWebPath(FileInfo file)
		{
			// ReSharper disable PossibleNullReferenceException
			var courseDir = file.Directory.Parent.Name;
			var unitDir = file.Directory.Name;
			// ReSharper restore PossibleNullReferenceException
			return courseDir + "/" + unitDir;
		}
	}
}
