using System.IO;

namespace uLearn
{
	public interface ISlideLoader
	{
		string Extension { get; }
		Slide Load(FileInfo file, Unit unit, int slideIndex, CourseSettings settings);
	}
}