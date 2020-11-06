import api from "src/api/index";

export function submitCode(courseId, slideId, code) {
	return api.post(`slides/${ courseId }/${ slideId }/exercise/submit`,
		api.createRequestParams({ solution: code }));
}

export function addCodeReviewComment(reviewId, text) {
	return api.post(`review/${ reviewId }/comments`,
		api.createRequestParams({ text }));
}
