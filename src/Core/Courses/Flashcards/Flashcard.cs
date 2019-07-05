using Ulearn.Core.Courses.Slides.Blocks;

namespace Ulearn.Core.Courses.Flashcards
{
	public class Flashcard : IFlashcard
	{
		public string Id { get; }
		
		public FlashcardBlock[] QuestionBlocks { get; }
		public FlashcardBlock[] AnswerBlocks { get; }
	}
}