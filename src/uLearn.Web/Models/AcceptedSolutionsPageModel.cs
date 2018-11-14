using System.Collections.Generic;
using System.Security.Principal;
using uLearn.Courses.Slides;

namespace uLearn.Web.Models
{
	public class AcceptedSolutionsPageModel
	{
		public string CourseId;
		public string CourseTitle;
		public ExerciseSlide Slide;
		public List<AcceptedSolutionInfo> AcceptedSolutions;
		public IPrincipal User;
		public string LikeSolutionUrl;
		public bool IsLti;
		public bool IsPassed;
	}
}