using System.Collections.Generic;

namespace uLearn.Web.Models
{
	public class QuizAnswerInfo
	{
		public string Id { get; set; }
		public bool IsRight { get; set; }
		public int Score { get; set; }
		public int MaxScore { get; set; }
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
		public HashSet<string> CorrectAnswer { get; set; }
	}

	public class OrderingBlockAnswerInfo : QuizAnswerInfo
	{
		public List<int> AnswersPositions { get; set; }
	}

	public class MatchingBlockAnswerInfo : QuizAnswerInfo
	{
		public List<bool> IsRightMatches { get; set; }
	}
}