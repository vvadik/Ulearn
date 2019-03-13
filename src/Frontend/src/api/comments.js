import api from "./index";

export function getComments(courseId, slideId, isForInstructors) {
	return api.get("comments?course_id=" + courseId + "&slide_id=" + slideId + "&for_instructors="
		+ encodeURIComponent(isForInstructors));
}

export function getComment(commentId) {
	return api.get("comments/" + commentId);
}

export function addComment(courseId, slideId, text, parentId, forInstructors) {
	return api.post("comments?course_id=" + courseId,
		api.createRequestParams({
			"slideId": slideId,
			"text": text,
			"parentCommentId": parentId,
			"forInstructors": forInstructors,
		})
	);
}

export function deleteComment(commentId) {
	return api.delete("comments/" + commentId);
}

export function updateComment(commentId, commentSettings) {
	return api.patch("comments/" + commentId,
		api.createRequestParams(commentSettings)
	);
}

export function likeComment(commentId) {
	return api.post("comments/" + commentId + "/like");
}

export function dislikeComment(commentId) {
	return api.delete("comments/" + commentId + "/like");
}

