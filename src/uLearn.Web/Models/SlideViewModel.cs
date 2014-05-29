namespace uLearn.Web.Models
{
	public class SlideViewModel
	{
		public Slide Slide { get; set; }
		public Course Course { get; set; }

		public SlideViewModel(Slide slide, Course course)
		{
			Slide = slide;
			Course = course;
		}
	}
}