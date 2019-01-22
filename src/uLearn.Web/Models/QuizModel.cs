using System.Collections.Generic;
using System.Linq;
using Database.Models;
using Ulearn.Common;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides.Quizzes;

namespace uLearn.Web.Models
{
	public class QuizModel
	{
		public Course Course { get; set; }

		public QuizSlide Slide { get; set; }
		public QuizState QuizState { get; set; }
		public Dictionary<string, List<UserQuizAnswer>> AnswersToQuizes { get; set; }
		public Dictionary<string, int> ScoresForBlocks { get; set; }

		/* (quizId -> (itemId -> frequency%)) */
		public DefaultDictionary<string, DefaultDictionary<string, int>> QuestionAnswersFrequency { get; set; } = new DefaultDictionary<string, DefaultDictionary<string, int>>();

		public int TryNumber { get; set; }
		public int MaxTriesCount { get; set; }
		public bool IsLti { get; set; }
		public bool IsGuest { get; set; }
		public ManualQuizChecking Checking { get; set; }
		public bool CanUserFillQuiz { get; set; }

		/* GroupsIds != null if instructor filtered users by group and see their works */
		public List<string> GroupsIds { get; set; }

		public string GroupsIdsJoined => string.Join(",", GroupsIds ?? new List<string>());

		public int Score
		{
			get { return ScoresForBlocks?.AsEnumerable().Sum(res => res.Value) ?? 0; }
		}

		public int QuestionsCount => ScoresForBlocks?.Count ?? 0;

		public string IsLtiToString => IsLti.ToString().ToLowerInvariant();
	}
}