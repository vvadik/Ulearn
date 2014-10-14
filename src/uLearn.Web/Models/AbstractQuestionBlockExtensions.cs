using System.Collections.Generic;
using System.Linq;
using uLearn.Quizes;

namespace uLearn.Web.Models
{
	public static class AbstractQuestionBlockExtensions
	{
		public static bool IsCorrectAnswer(this AbstractQuestionBlock questionBlock,  CoursePageModel pageModel)
		{
			return IsCorrectAnswerForType((dynamic)questionBlock, pageModel.AnswersToQuizes);
		}

		private static bool IsCorrectAnswerForType(FillInBlock fillInBlock, Dictionary<string, List<string>> answersToQuizes)
		{
			return answersToQuizes[fillInBlock.Id][1] == "True";
		}

		private static bool IsCorrectAnswerForType(ChoiceBlock choiceBlock, Dictionary<string, List<string>> answersToQuizes)
		{
			var itemsCorrect = new HashSet<string>(choiceBlock.Items.Where(item => item.IsCorrect).Select(item => item.Id));
			var itemsChecked = new HashSet<string>(answersToQuizes[choiceBlock.Id]);

			return itemsCorrect.SetEquals(itemsChecked);
		}

		private static bool IsCorrectAnswerForType(IsTrueBlock isTrueBlock, Dictionary<string, List<string>> answersToQuizes)
		{
			var userAnswer = answersToQuizes[isTrueBlock.Id].FirstOrDefault() == "True";
			var correctAnswer = isTrueBlock.Answer;

			return userAnswer == correctAnswer;
		}

	}
}