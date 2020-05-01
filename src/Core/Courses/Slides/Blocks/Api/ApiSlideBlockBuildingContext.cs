using System;
using System.IO;

namespace Ulearn.Core.Courses.Slides.Blocks.Api
{
	public class ApiSlideBlockBuildingContext
	{
		public string CourseId;
		public Guid SlideId;
		public string BaseUrl;
		public bool RemoveHiddenBlocks;
		public DirectoryInfo UnitDirectory;

		public ApiSlideBlockBuildingContext(string courseId, Guid slideId, string baseUrl, bool removeHiddenBlocks,
			DirectoryInfo unitDirectory)
		{
			CourseId = courseId;
			SlideId = slideId;
			BaseUrl = baseUrl;
			RemoveHiddenBlocks = removeHiddenBlocks;
			UnitDirectory = unitDirectory;
		}
	}
}