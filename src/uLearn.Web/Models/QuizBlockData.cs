namespace uLearn.Web.Models
{
	public class QuizBlockData
	{
		public QuizModel QuizModel {get; set;}
		public int BlockIndex { get; set; }
		public QuizState QuizState;

		public QuizBlockData(QuizModel model, int index, QuizState quizState)
		{
			QuizModel = model;
			BlockIndex = index;
			QuizState = quizState;
		}

		public bool ShowCorrectAnswer
		{
			get { return QuizState == QuizState.Total && !QuizModel.Slide.Quiz.ManualCheck; }
		}
	}

}
