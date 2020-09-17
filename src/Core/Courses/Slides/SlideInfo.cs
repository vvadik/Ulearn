using System.IO;
using Ulearn.Core.Courses.Units;

namespace Ulearn.Core.Courses.Slides
{
	public class SlideInfo
	{
		public Unit Unit { get; private set; }
		public FileInfo SlideFile { get; set; }
		public DirectoryInfo Directory => SlideFile.Directory;

		public SlideInfo(Unit unit, FileInfo slideFile)
		{
			Unit = unit;
			SlideFile = slideFile;
		}

		public string DirectoryRelativePath => CourseUnitUtils.GetDirectoryRelativeWebPath(SlideFile);
	}
}