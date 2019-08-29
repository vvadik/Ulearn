export const coursePath = "course";
export const flashcards = "flashcards";
export const courseStatistics = "/analytics/courseStatistics";

export function constructPathToSlide(courseId, slideId) {
	return `/${ coursePath }/${ courseId }/${ slideId }`;
}
