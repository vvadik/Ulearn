using System.Collections.Generic;
using System.Linq;

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
		public QuizState QuizState { get; set; }
		public Dictionary<string, List<string>> AnswersToQuizes { get; set; }
		public Dictionary<string, bool> ResultsForQuizes { get; set; }

		public int RightAnswers
		{
			get { return ResultsForQuizes == null ? 0 : ResultsForQuizes.AsEnumerable().Count(res => res.Value); }
		}
		public int QuestionsCount {
			get { return ResultsForQuizes == null ? 0 : ResultsForQuizes.Count; }
		}

	}
}