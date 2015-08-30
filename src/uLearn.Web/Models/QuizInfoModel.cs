namespace uLearn.Web.Models
{
	public class QuizInfoModel
	{
		public QuizModel QuizModel {get; set;}
		public int BlockIndex { get; set; }
		public QuizState QuizState;

		public QuizInfoModel(QuizModel model, int index, QuizState quizState)
		{
			QuizModel = model;
			BlockIndex = index;
			QuizState = quizState;
		}
	}

}
