using System;

namespace uLearn.Web.Models
{
	public class CoursePageModel
	{
		public bool IsFirstCourseVisit { get; set; }
		public string CourseId;
		public string UserId { get; set; }
		public string CourseTitle;
		public Slide Slide;
		public string Rate {get; set;}
		public Tuple<int, int> Score { get; set; }
		public BlockRenderContext BlockRenderContext { get; set; }
	}
}