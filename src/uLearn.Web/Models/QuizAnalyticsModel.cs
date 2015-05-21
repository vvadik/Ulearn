using System.Collections.Generic;
using uLearn.Quizes;

namespace uLearn.Web.Models
{
	public class QuizAnalyticsModel
	{
		public SortedDictionary<string,List<QuizAnswerInfo>> UserAnswers { get; set; }
		public QuizSlide QuizSlide { get; set; }
		public Dictionary<string, int> RightAnswersCount { get; set; }
		public Dictionary<string, string> Group { get; set; }
	}

	public class QuizAnswerInfo
	{
		public string Id { get; set; }
		public bool IsRight { get; set; }
	}

	public class FillInBlockAnswerInfo : QuizAnswerInfo
	{
		public string Answer { get; set; }
	}

	public class IsTrueBlockAnswerInfo : QuizAnswerInfo
	{
		public bool Answer { get; set; }
		public bool IsAnswered { get; set; }
	}

	public class ChoiceBlockAnswerInfo : QuizAnswerInfo
	{
		public SortedDictionary<string, bool> AnswersId { get; set; }
		public HashSet<string> RealyRightAnswer { get; set; }
	}
}
