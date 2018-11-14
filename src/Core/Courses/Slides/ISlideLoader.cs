using System.IO;

namespace uLearn.Courses.Slides
{
	public interface ISlideLoader
	{
		string Extension { get; }
		Slide Load(FileInfo file, Unit unit, int slideIndex, string courseId, CourseSettings settings);
	}
}