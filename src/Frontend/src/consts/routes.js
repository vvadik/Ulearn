export const coursePath = "course";
export const flashcards = "flashcards";
export const commentsPath = "comments";
export const commentPoliciesPath = "comment-policies";
export const courseStatistics = "/analytics/courseStatistics";
export const slides = "slides";

export function constructPathToSlide(courseId, slideId) {
	return `/${ coursePath }/${ courseId }/${ slideId }`;
}

export function constructPathToComment(commentId, isLike) {
	const url = `${ commentsPath }/${ commentId }`;

	if (isLike) {
		return url + "/like";
	}

	return url;
}
