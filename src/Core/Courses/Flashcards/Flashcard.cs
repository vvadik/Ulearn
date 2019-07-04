using Ulearn.Core.Courses.Slides.Blocks;

namespace Ulearn.Core.Courses.Flashcards
{
	public class Flashcard : IFlashcard
	{
		public string Id { get; }
		
		public MarkdownBlock[] QuestionBlocks { get; }
		public MarkdownBlock[] AnswerBlocks { get; }
	}
}