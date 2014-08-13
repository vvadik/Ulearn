using System.Collections.Generic;
using uLearn.Web.Models;

namespace uLearn
{
	public class AcceptedSolutionsPageModel
	{
		public string CourseId;
		public string CourseTitle;
		public Slide Slide;
		public List<AcceptedSolutionInfo> AcceptedSolutions;
	}
}
