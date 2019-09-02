export default function getFlashcardsWithTheorySlides(infoByUnits, courseFlashcards, courseSlides) {
	const flashcards = [];

	for (const { flashcardsIds } of infoByUnits) {
		flashcards.push(...getUnitFlashcardsWithTheorySlides(flashcardsIds, courseFlashcards, courseSlides));
	}

	return flashcards;
}

function getUnitFlashcardsWithTheorySlides(flashcardsIds, courseFlashcards, courseSlides) {
	const flashcards = [];

	for (const id of flashcardsIds) {
		const flashcard = { ...courseFlashcards[id] };
		const { theorySlidesIds } = flashcard;
		const theorySlides = getTheorySlides(theorySlidesIds, courseSlides);

		flashcards.push({
			...flashcard,
			theorySlides,
		});
	}

	return flashcards;
}

function getTheorySlides(theorySlidesIds, courseSlides) {
	const theorySlides = [];

	for (const slideId of theorySlidesIds) {
		const courseSlide = courseSlides.find(slide => slide.id === slideId);

		if (courseSlide) {
			theorySlides.push({
				slug: courseSlide.slug,
				title: courseSlide.title,
			});
		}
	}

	return theorySlides;
}
