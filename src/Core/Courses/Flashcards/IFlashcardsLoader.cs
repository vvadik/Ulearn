using System.Collections.Generic;

namespace Ulearn.Core.Courses.Flashcards
{
	public interface IFlashcardsLoader
	{
		List<Flashcards> Load(FlashcardLoadingContext context);
	}
}