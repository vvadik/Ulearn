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
			MaxScore = Blocks.Count(block => block is AbstractQuestionBlock);
			QuizNormalizedXml = quiz.NormalizedXml;
		}

		public override bool ShouldBeSolved { get { return true; } }

		public int MaxDropCount { get; private set; }

		public string QuizNormalizedXml { get; private set; }

		public AbstractQuestionBlock GetBlockById(string id)
		{
			return Blocks.OfType<AbstractQuestionBlock>().FirstOrDefault(block => block.Id == id);
		}
	}
}
