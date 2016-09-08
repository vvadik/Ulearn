using System;
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
		public Dictionary<string, List<UserQuiz>> AnswersToQuizes { get; set; }
		public Dictionary<string, int> ResultsForQuizes { get; set; }
		public int TryNumber { get; set; }
		public int MaxDropCount { get; set; }
		public bool IsLti { get; set; }
		public bool IsGuest { get; set; }
		public ManualQuizChecking ManualQuizCheckQueueItem { get; set; }
		public bool CanUserFillQuiz { get; set; }

		/* GroupId != null if instructor filtered users by group and see their works */
		public int? GroupId { get; set; }

		public int Score
		{
			get { return ResultsForQuizes == null ? 0 : ResultsForQuizes.AsEnumerable().Sum(res => res.Value); }
		}

		public int QuestionsCount
		{
			get { return ResultsForQuizes == null ? 0 : ResultsForQuizes.Count; }
		}

		public string IsLtiToString
		{
			get { return IsLti.ToString().ToLowerInvariant(); }
		}
	}
}