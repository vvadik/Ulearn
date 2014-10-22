using uLearn.Quizes;

namespace uLearn.Web.Models
{
	public class QuizInfoModel
	{
		public QuizModel QuizModel {get; set;}
		public QuizBlock CurrentBlock { get; set;}
		public int BlockIndex { get; set; }
		public QuizState QuizState;

		public QuizInfoModel(QuizModel model, QuizBlock currentBlock, int index, QuizState quizState)
		{
			QuizModel = model;
			CurrentBlock = currentBlock;
			BlockIndex = index;
			QuizState = quizState;
		}
	}

}
