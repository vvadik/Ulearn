namespace uLearn.Web.Models
{
	public class QuizBlockData
	{
		public QuizModel QuizModel { get; private set; }
		public int BlockIndex { get; private set; }
		public QuizState QuizState { get; private set; }

		public QuizBlockData(QuizModel model, int index, QuizState quizState)
		{
			QuizModel = model;
			BlockIndex = index;
			QuizState = quizState;
		}

		private bool TriesFinished => QuizModel.TryNumber + 1 > QuizModel.MaxTriesCount; 

		public bool ShowCorrectAnswers => (QuizState == QuizState.Total || QuizState == QuizState.Subtotal && TriesFinished) && !QuizModel.Slide.ManualChecking;

		public bool ShowExplanations => ShowCorrectAnswers;
	}
}