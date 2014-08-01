using System.Collections.Generic;

namespace uLearn
{
	public class CoursePageModel
	{
		public Course Course;
		public Slide Slide;
		public int SlideIndex;
		public int NextSlideIndex;
		public int PrevSlideIndex;
		public bool HasNextSlide { get { return NextSlideIndex < Course.Slides.Length; } }
		public bool HasPrevSlide { get { return PrevSlideIndex >= 0; } }
		public bool IsPassedTask { get; set; }
		public string LatestAcceptedSolution { get; set; }
		public string Rate {get; set;}
		public HashSet<string> VisitedSlide { get; set; }
		public HashSet<string> SolvedSlide { get; set; }
	}
}