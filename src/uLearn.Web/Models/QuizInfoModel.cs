using uLearn.Quizes;

namespace uLearn.Web.Models
{
	public class QuizInfoModel
	{
		public CoursePageModel CourseModel {get; set;}
		public QuizBlock CurrentBlock { get; set;}
		public int BlockIndex { get; set; }
		public QuizState QuizState;

		public QuizInfoModel(CoursePageModel model, QuizBlock currentBlock, int index, QuizState quizState)
		{
			CourseModel = model;
			CurrentBlock = currentBlock;
			BlockIndex = index;
			QuizState = quizState;
		}
	}

}
