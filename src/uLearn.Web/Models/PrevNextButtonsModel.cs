using System;

namespace uLearn.Web.Models
{
	public class PrevNextButtonsModel
	{
		public PrevNextButtonsModel(Course course, Guid slideId, bool nextIsAcceptedSolutions, Slide nextSlide, Slide prevSlide, bool isGuest)
		{
			this.course = course;
			SlideId = slideId;
			NextIsAcceptedSolutions = nextIsAcceptedSolutions;
			NextSlide = nextSlide;
			PrevSlide = prevSlide;
			IsGuest = isGuest;
		}

		private readonly Course course;
		public string CourseId => course.Id;

		public bool NextIsAcceptedSolutions { get; set; }

		public Slide NextSlide { get; private set; }
		public Slide PrevSlide { get; private set; }

		public bool HasNextSlide => NextSlide != null;

		public bool HasPrevSlide => PrevSlide != null;

		public bool IsGuest { get; set; }
		public Guid SlideId { get; set; }

		public void SetPrevSlide(Slide slide)
		{
			PrevSlide = slide;
		}
	}
}