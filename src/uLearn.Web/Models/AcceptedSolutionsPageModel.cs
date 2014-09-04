using System.Collections.Generic;

namespace uLearn.Web.Models
{
	public class AcceptedSolutionsPageModel
	{
		public string CourseId;
		public string CourseTitle;
		public ExerciseSlide Slide;
		public List<AcceptedSolutionInfo> AcceptedSolutions;
	}
}
