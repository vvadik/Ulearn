using System.Collections.Generic;
using System.Linq;
using uLearn.Quizes;

namespace uLearn.Web.Models
{
	public class QuizModel
	{
		public string CourseId { get; set; }

		public QuizSlide Slide { get; set; }
		public QuizState QuizState { get; set; }
		public Dictionary<string, List<string>> AnswersToQuizes { get; set; }
		public Dictionary<string, bool> ResultsForQuizes { get; set; }
		public int TryNumber { get; set; }
		public int MaxDropCount { get; set; }

		public int RightAnswers
		{
			get { return ResultsForQuizes == null ? 0 : ResultsForQuizes.AsEnumerable().Count(res => res.Value); }
		}
		public int QuestionsCount
		{
			get { return ResultsForQuizes == null ? 0 : ResultsForQuizes.Count; }
		}
	}
}