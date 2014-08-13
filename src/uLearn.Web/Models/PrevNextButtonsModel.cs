namespace uLearn
{
	public class PrevNextButtonsModel
	{
		public PrevNextButtonsModel(Course course, int slideIndex, bool isPassedTask, bool onSolutionsSlide)
		{
			this.course = course;
			SlideIndex = slideIndex;
			NextSlideIndex = slideIndex + 1;
			PrevSlideIndex = slideIndex - 1;
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
		public bool HasNextSlide { get { return NextSlideIndex < course.Slides.Length; } }
		public bool HasPrevSlide { get { return PrevSlideIndex >= 0; } }
	}
}