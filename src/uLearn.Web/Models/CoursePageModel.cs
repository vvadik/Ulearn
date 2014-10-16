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
		public QuizState QuizState;
		public Dictionary<string, List<string>> AnswersToQuizes { get; set; }
	}
}