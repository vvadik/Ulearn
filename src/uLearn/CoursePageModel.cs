namespace uLearn
{
	public class CoursePageModel
	{
		public Course Course;
		public Slide Slide;
		public int SlideIndex;

		public string SlideClass
		{
			get { return Slide is ExerciseSlide ? "exercise" : "theory"; }
		}
	}
}