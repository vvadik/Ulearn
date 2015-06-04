using uLearn.Model.Blocks;

namespace uLearn.Web.Models
{
	public class QuizInfoModel
	{
		public QuizModel QuizModel {get; set;}
		public SlideBlock CurrentBlock { get; set;}
		public int BlockIndex { get; set; }
		public QuizState QuizState;

		public QuizInfoModel(QuizModel model, SlideBlock currentBlock, int index, QuizState quizState)
		{
			QuizModel = model;
			CurrentBlock = currentBlock;
			BlockIndex = index;
			QuizState = quizState;
		}
	}

}
