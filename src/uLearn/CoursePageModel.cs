namespace uLearn
{
	public class CoursePageModel
	{
		public Course Course;
		public Slide Slide;
		public int SlideIndex;
		public bool HasNextSlide { get { return SlideIndex < Course.Slides.Length - 1; } }
		public bool HasPrevSlide { get { return SlideIndex > 0; } }

		public string SlideClass
		{
			get { return Slide is ExerciseSlide ? "exercise" : "theory"; }
		}
	}
}