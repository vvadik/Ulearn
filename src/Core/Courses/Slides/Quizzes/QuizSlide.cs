using System;
using System.Linq;

namespace uLearn.Courses.Slides.Quizzes
{
	public class QuizSlide : Slide
	{
		public QuizSlide(SlideInfo slideInfo, Quiz quiz)
			: base(quiz.Blocks, slideInfo, quiz.Title, Guid.Parse(quiz.Id), quiz.Meta)
		{
			/* Backward compatibility: don't use MaxDropCount now, use MaxTriesCount instead of it */
			const int defaultMaxDropCount = 1;
			MaxTriesCount = quiz.MaxTriesCount > 0 ? quiz.MaxTriesCount : ((quiz.MaxDropCount == 0 ? defaultMaxDropCount: quiz.MaxDropCount) + 1);
			MaxScore = quiz.MaxScore;
			ManualChecking = quiz.ManualChecking;
			QuizNormalizedXml = quiz.NormalizedXml;
			ScoringGroup = quiz.ScoringGroup ?? "";
		}

		public override bool ShouldBeSolved => true;

		public int MaxTriesCount { get; private set; }

		public bool ManualChecking { get; private set; }

		public string QuizNormalizedXml { get; private set; }

		public AbstractQuestionBlock GetBlockById(string id)
		{
			return Blocks.OfType<AbstractQuestionBlock>().FirstOrDefault(block => block.Id == id);
		}
	}
}