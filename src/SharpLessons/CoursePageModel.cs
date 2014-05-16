namespace SharpLessons
{
	public class CoursePageModel
	{
		public string CourseName;
		public string NextSlideId;
		public string PrevSlideId;
		public Slide Slide;

		public string SlideClass
		{
			get { return Slide is ExerciseSlide ? "exercise" : "theory"; }
		}
	}
}