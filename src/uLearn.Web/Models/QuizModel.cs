using System.Collections.Generic;
using System.Linq;
using Database.Models;
using uLearn.Quizes;
using Ulearn.Common;

namespace uLearn.Web.Models
{
	public class QuizModel
	{
		public Course Course { get; set; }

		public QuizSlide Slide { get; set; }
		public QuizState QuizState { get; set; }
		public Dictionary<string, List<UserQuiz>> AnswersToQuizes { get; set; }
		public Dictionary<string, int> ResultsForQuizes { get; set; }

		/* (quizId -> (itemId -> frequency%)) */
		public DefaultDictionary<string, DefaultDictionary<string, int>> QuestionAnswersFrequency { get; set; }

		public int TryNumber { get; set; }
		public int MaxTriesCount { get; set; }
		public bool IsLti { get; set; }
		public bool IsGuest { get; set; }
		public ManualQuizChecking ManualQuizCheckQueueItem { get; set; }
		public bool CanUserFillQuiz { get; set; }

		/* GroupsIds != null if instructor filtered users by group and see their works */
		public List<string> GroupsIds { get; set; }

		public string GroupsIdsJoined => string.Join(",", GroupsIds ?? new List<string>());

		public int Score
		{
			get { return ResultsForQuizes?.AsEnumerable().Sum(res => res.Value) ?? 0; }
		}

		public int QuestionsCount => ResultsForQuizes?.Count ?? 0;

		public string IsLtiToString => IsLti.ToString().ToLowerInvariant();
	}
}