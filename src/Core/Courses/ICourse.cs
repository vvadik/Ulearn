using System;
using System.Collections.Generic;
using System.IO;
using uLearn.Courses.Slides;

namespace uLearn.Courses
{
	public interface ICourse
	{
		string Id { get; set; }
		string Title { get; }
		CourseSettings Settings { get; }
		DirectoryInfo Directory { get; }
		List<Unit> Units { get; }
		List<Slide> Slides { get; }
		
		Slide FindSlideById(Guid slideId);
		Slide GetSlideById(Guid slideId);
		Slide FindSlideByIndex(int index);
		Unit FindUnitById(Guid unitId);
		Unit GetUnitById(Guid unitId);
		Unit FindUnitBySlideId(Guid slideId);
	}
}