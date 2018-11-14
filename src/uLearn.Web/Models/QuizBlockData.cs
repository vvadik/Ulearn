using Ulearn.Common;

namespace uLearn.Web.Models
{
	public class QuizBlockData
	{
		public QuizModel QuizModel { get; private set; }
		public int BlockIndex { get; private set; }
		public QuizState QuizState { get; private set; }
		public DefaultDictionary<string, int> QuestionAnswersFrequency { get; private set; }
		public readonly bool IsInstructor;
		public readonly bool IsCourseAdmin;
		public readonly bool DebugView;

		public QuizBlockData(QuizModel model, int index, QuizState quizState, DefaultDictionary<string, int> questionAnswersFrequency=null, bool isInstructor=false, bool debugView=false, bool isCourseAdmin=false)
		{
			QuizModel = model;
			BlockIndex = index;
			QuizState = quizState;
			QuestionAnswersFrequency = questionAnswersFrequency ?? new DefaultDictionary<string, int>();
			IsCourseAdmin = isCourseAdmin;
			IsInstructor = isInstructor;
			DebugView = debugView;
		}

		private bool TriesFinished => QuizModel.TryNumber + 1 > QuizModel.MaxTriesCount; 

		public bool ShowCorrectAnswers
		{
			get
			{
				if (QuizModel.Slide.ManualChecking)
					return false;
				if (QuizState == QuizState.Total)
					return true;
				if (QuizState == QuizState.Subtotal)
					return TriesFinished || IsInstructor;
				return false;
			}
		}

		public bool ShowExplanations => ShowCorrectAnswers;
		public bool ShowQuestionStatistics => !DebugView && IsCourseAdmin;
	}
}