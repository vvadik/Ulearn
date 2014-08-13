using System.Linq;
using System.Text;
using uLearn.Quizes;

namespace uLearn
{
	public class QuizSlide : Slide
	{
		public QuizSlide(SlideInfo slideInfo, Quiz quiz)
			: base(new SlideBlock[0], slideInfo, quiz.Title, quiz.Id)
		{
			Quiz = quiz;
		}

		public override bool ShouldBeSolved { get { return true; } }

		public Quiz Quiz { get; set; }

		public QuizBlock GetBlockById(string id)
		{
			return Quiz.Blocks.FirstOrDefault(block => block.Id == id);
		}

		public string RightAnswersToQuiz
		{
			get { return GetRightAnswerToQuiz(); }
		}

		private string GetRightAnswerToQuiz()
		{
			var rightAnswersStr = new StringBuilder();
			foreach (var quizBlock in Quiz.Blocks)
			{
				if (quizBlock is ChoiceBlock)
				{
					var choiceQuizBlock = quizBlock as ChoiceBlock;
					rightAnswersStr.Append(quizBlock.Id + "quizBlock" + "=");
					rightAnswersStr.Append(string.Join("*", choiceQuizBlock.Items.Where(x => x.IsCorrect).Select(x => x.Id).ToList()));
					rightAnswersStr.Append("||");
				}
				else if (quizBlock is FillInBlock)
				{
					var fillIn = quizBlock as FillInBlock;
					rightAnswersStr.Append(quizBlock.Id + quizBlock.Id + "=" + fillIn.Sample);
					rightAnswersStr.Append("||");
				}
				else if (quizBlock is IsTrueBlock)
				{
					var selecter = quizBlock as IsTrueBlock;
					rightAnswersStr.Append(quizBlock.Id + "quizBlock" + "=" + selecter.Answer);
					rightAnswersStr.Append("||");
				}
			}
			return rightAnswersStr.ToString();
		}
	}
}
