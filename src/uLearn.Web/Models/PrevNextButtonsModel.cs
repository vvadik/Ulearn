namespace uLearn.Web.Models
{
	public class PrevNextButtonsModel
	{
		public PrevNextButtonsModel(Course course, int slideIndex, bool nextIsAcceptedSolutions, int nextSlide, int prevSlide, bool isGuest)
		{
			this.course = course;
			SlideIndex = slideIndex;
			NextIsAcceptedSolutions = nextIsAcceptedSolutions;
			NextSlideIndex = nextSlide;
			PrevSlideIndex = prevSlide;
			IsGuest = isGuest;
		}
		private readonly Course course;
	
		public int SlideIndex { get; set; }
		public bool NextIsAcceptedSolutions { get; set; }
		public string CourseId { get { return course.Id; }}

		public int NextSlideIndex;
		public int PrevSlideIndex;
		public bool HasNextSlide { get { return NextSlideIndex >= 0; } }
		public bool HasPrevSlide { get { return PrevSlideIndex >= 0; } }

		public bool IsGuest { get; set; }
	}
}