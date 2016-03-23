namespace uLearn.Web.Models
{
	public class PrevNextButtonsModel
	{
		public PrevNextButtonsModel(Course course, int slideIndex, bool nextIsAcceptedSolutions, Slide nextSlide, Slide prevSlide, bool isGuest)
		{
			this.course = course;
			SlideIndex = slideIndex;
			NextIsAcceptedSolutions = nextIsAcceptedSolutions;
			NextSlide = nextSlide;
			PrevSlide = prevSlide;
			IsGuest = isGuest;
		}
		private readonly Course course;
	
		public int SlideIndex { get; set; }
		public bool NextIsAcceptedSolutions { get; set; }
		public string CourseId { get { return course.Id; }}

		public Slide NextSlide;
		public Slide PrevSlide;
		public bool HasNextSlide { get { return NextSlide != null; } }
		public bool HasPrevSlide { get { return PrevSlide != null;  } }

		public bool IsGuest { get; set; }
	}
}