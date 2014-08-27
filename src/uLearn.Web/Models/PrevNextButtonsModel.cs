namespace uLearn.Web.Models
{
	public class PrevNextButtonsModel
	{
		public PrevNextButtonsModel(Course course, int slideIndex, bool isPassedTask, bool onSolutionsSlide, int nextSlide, int prevSlide)
		{
			this.course = course;
			SlideIndex = slideIndex;
			NextSlideIndex = nextSlide;
			PrevSlideIndex = prevSlide;
			IsPassedTask = isPassedTask;
			OnSolutionsSlide = onSolutionsSlide;
		}
		private readonly Course course;
	
		public readonly bool IsPassedTask;
		public readonly bool OnSolutionsSlide;
		public int SlideIndex { get; set; }
		public string CourseId { get { return course.Id; }}

		public int NextSlideIndex;
		public int PrevSlideIndex;
		public bool HasNextSlide { get { return NextSlideIndex >= 0; } }
		public bool HasPrevSlide { get { return PrevSlideIndex >= 0; } }
	}
}