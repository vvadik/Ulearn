using System;
using System.Linq;

namespace uLearn.Quizes
{
	public class QuizSlide : Slide
	{
		public QuizSlide(SlideInfo slideInfo, Quiz quiz)
			: base(quiz.Blocks, slideInfo, quiz.Title, Guid.Parse(quiz.Id))
		{
			MaxDropCount = quiz.MaxDropCount;
			MaxScore = Blocks.OfType<AbstractQuestionBlock>().Sum(block => block.MaxScore);
			Quiz = quiz;
		}

		public override bool ShouldBeSolved { get { return true; } }

		public int MaxDropCount { get; private set; }

		public Quiz Quiz { get; private set; }

		public string QuizNormalizedXml
		{
			get { return Quiz.NormalizedXml; }
		}

		public AbstractQuestionBlock GetBlockById(string id)
		{
			return Blocks.OfType<AbstractQuestionBlock>().FirstOrDefault(block => block.Id == id);
		}
	}
}
