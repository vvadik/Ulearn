using System.IO;

namespace uLearn
{
	public interface ISlideLoader
	{
		string Extension { get; }
		Slide Load(FileInfo file, string unitName, int slideIndex, CourseSettings settings);
	}
}