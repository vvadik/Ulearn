using System.Collections.Generic;

namespace uLearn.Web.Models
{
	public class CoursePageModel
	{
		public bool IsFirstCourseVisit { get; set; }
		public string CourseId;
		public string CourseTitle;
		public Slide Slide;
		public string LatestAcceptedSolution { get; set; }
		public string Rate {get; set;}
		public HashSet<string> PassedQuiz { get; set; }
		public Dictionary<string, List<string>> AnswersToQuizes { get; set; }
	}
}