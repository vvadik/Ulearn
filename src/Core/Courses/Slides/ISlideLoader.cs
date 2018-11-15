using System.IO;
using Ulearn.Core.Courses.Units;

namespace Ulearn.Core.Courses.Slides
{
	public interface ISlideLoader
	{
		string Extension { get; }
		Slide Load(FileInfo file, Unit unit, int slideIndex, string courseId, CourseSettings settings);
	}
}