export const coursePath = "course";
export const flashcards = "flashcards";
export const unitFlashcardsSlideSlug = "voprosy_dlya_samoproverki";

export function constructPathToSlide(courseId, slideId) {
	return `/${ coursePath }/${ courseId }/${ slideId }`;
}
