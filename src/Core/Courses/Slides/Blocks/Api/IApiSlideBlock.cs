using System.Collections.Generic;

namespace Ulearn.Core.Courses.Slides.Blocks.Api
{
	// Маркирует блоки, коотрые могут возвращаться из api в видe json напрямую
	public interface IApiSlideBlock
	{
		bool Hide { get; set; }
		string Type { get; set; }
	}
	
	public interface IApiConvertibleSlideBlock
	{
		IEnumerable<IApiSlideBlock> ToApiSlideBlocks(ApiSlideBlockBuildingContext context);
	}
}