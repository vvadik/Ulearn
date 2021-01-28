import api from "src/api/index";

export function submitCode(courseId, slideId, code, language) {
	return api.post(`slides/${ courseId }/${ slideId }/exercise/submit?language=${language}`,
		api.createRequestParams({ solution: code }));
}

export function sendCodeReviewComment(reviewId, text) {
	return api.post(`review/${ reviewId }/comments`,
		api.createRequestParams({ text }));
}

export function deleteCodeReviewComment(reviewId, commentId) {
	return api.delete(`review/${ reviewId }/comments/${ commentId }`);
}
