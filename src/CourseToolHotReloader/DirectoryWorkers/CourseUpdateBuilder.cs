using System.IO;
using CourseToolHotReloader.Dtos;

namespace CourseToolHotReloader.DirectoryWorkers
{
	public class CourseUpdateBuilder
	{
		public static CourseUpdate Build(FileSystemEventArgs fileSystemEventArgs)
		{
			var courseUpdate = new CourseUpdate
			{
				Name = fileSystemEventArgs.Name,
				RelativePath = fileSystemEventArgs.FullPath
			};
			return courseUpdate;
		}
	}
}