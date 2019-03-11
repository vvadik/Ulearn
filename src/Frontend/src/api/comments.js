import api from "./index";

export function getComments(courseId, slideId, isForInstructors) {
	return api.get("/comments/in/" + courseId + "/" + slideId + "?for_instructors="
		+ encodeURIComponent(isForInstructors));
}

export function addComment(courseId, slideId, text, forInstructors, parentId) {
	return api.post("/comments/in/" + courseId + "/" + slideId,
		api.createRequestParams({
			"slideId": slideId,
			"text": text,
			"parentCommentId": parentId,
			"forInstructors": forInstructors,
		})
	);
}

export function deleteComment(commentId) {
	return api.delete("/comments/" + commentId);
}

export function updateComment(commentId, text, isApproved, isPinnedToTop, isCorrectAnswer) {
	return api.patch("/comments/" + commentId,
		api.createRequestParams({
			"text": text,
			"is_approved": isApproved,
			"is_pinned_to_top": isPinnedToTop,
			"is_correct_answer": isCorrectAnswer,
		})
	);
}

export function likeComment(commentId) {
	return api.post("/comments/" + commentId + "/like");
}

export function dislikeComment(commentId) {
	return api.delete("/comments/" + commentId + "/like");
}

