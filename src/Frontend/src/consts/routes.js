export const coursePath = "course";
export const flashcards = "flashcards";

export function constructPathToSlide(courseId, slideId) {
	return `/${coursePath}/${courseId}/${slideId}`;
}
