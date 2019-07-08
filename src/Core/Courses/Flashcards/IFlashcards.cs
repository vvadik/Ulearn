using System;

namespace Ulearn.Core.Courses.Flashcards
{
	public interface IFlashcards
	{
		Guid Id { get; }
		Flashcard[] FlashcardsList { get; }
	}
}