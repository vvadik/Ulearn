using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class QuizAnalyticsModel
	{
		public SortedDictionary<string,List<QuizAnswerInfo>> UserAnswers { get; set; }
		public QuizSlide QuizSlide { get; set; }
	}

	public class QuizAnswerInfo
	{
		public string Id { get; set; }
	}

	public class FillInBlockAnswerInfo : QuizAnswerInfo
	{
		public string Answer { get; set; }
	}

	public class IsTrueBlockAnswerInfo : QuizAnswerInfo
	{
		public bool Answer { get; set; }
	}

	public class ChoiseBlockAnswerInfo : QuizAnswerInfo
	{
		public SortedDictionary<string, bool> AnswersId { get; set; }
	}
}
