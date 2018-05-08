namespace uLearn.Web.Models
{
	public class QuizBlockData
	{
		public QuizModel QuizModel { get; private set; }
		public int BlockIndex { get; private set; }
		public QuizState QuizState { get; private set; }
		public readonly bool IsInstructor;		

		public QuizBlockData(QuizModel model, int index, QuizState quizState, bool isInstructor=false)
		{
			QuizModel = model;
			BlockIndex = index;
			QuizState = quizState;
			IsInstructor = isInstructor;			
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
	}
}