using System.IO;
using Ulearn.Core.Courses.Units;

namespace Ulearn.Core.Courses.Slides
{
	public interface ISlideLoader
	{
		Slide Load(FileInfo file, int slideIndex, Unit unit, string courseId, CourseSettings settings);
	}
}