using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Blocks;

namespace Ulearn.Core.Courses.Flashcards
{
	public interface IFlashcard
	{
		string Id { get; }

		//todo использовать другие блоки?
		FlashcardBlock[] QuestionBlocks { get; }
		FlashcardBlock[] AnswerBlocks { get; }
	}
}