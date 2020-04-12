using System.IO;
using CourseToolHotReloader.Dtos;

namespace CourseToolHotReloader.DirectoryWorkers
{
	public class CourseUpdateBuilder
	{
		public static ICourseUpdate Build(string name, string fullPath, string relativePath)
		{
			var courseUpdate = new CourseUpdate
			{
				Name = name,
				FullPath = fullPath,
				RelativePath = relativePath
			};
			return courseUpdate;
		}
	}
}