using System;
using System.Collections.Generic;

namespace uLearn.Web.Models
{
	public class TocModel
	{
		public string CourseId;
		public CourseUnitModel[] Units;
		public CourseUnitModel CurrentUnit;
		public Slide CurrentSlide;
		public HashSet<string> VisitedSlideIds { get; set; }
		public HashSet<string> SolvedSlideIds { get; set; }
		public DateTime NextUnitTime { get; set; }
	}

	public class CourseUnitModel
	{
		public string UnitName;
		public Slide[] Slides;
		public InstructorNote InstructorNote;
	}
}