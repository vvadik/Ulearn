using System;

namespace Ulearn.Core.Courses.Slides.Blocks.Api
{
	public class ApiSlideBlockBuildingContext
	{
		public string CourseId;
		public Guid SlideId;
		public string BaseUrl;
		public bool RemoveHiddenBlocks;

		public ApiSlideBlockBuildingContext(string courseId, Guid slideId, string baseUrl, bool removeHiddenBlocks)
		{
			CourseId = courseId;
			SlideId = slideId;
			BaseUrl = baseUrl;
			RemoveHiddenBlocks = removeHiddenBlocks;
		}
	}
}