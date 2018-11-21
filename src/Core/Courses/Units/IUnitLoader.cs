using System.IO;

namespace Ulearn.Core.Courses.Units
{
	public interface IUnitLoader
	{
		Unit LoadUnit(DirectoryInfo unitDir, CourseSettings courseSettings, string courseId, int firstSlideIndex);
	}
}