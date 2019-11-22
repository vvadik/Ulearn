export const coursePath = "course";
export const flashcards = "flashcards";
export const courseStatistics = "/analytics/courseStatistics";
export const acceptedSolutions = "acceptedsolutions";

export function constructPathToSlide(courseId, slideId) {
	return `/${ coursePath }/${ courseId }/${ slideId }`;
}
