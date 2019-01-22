import api from "../api/";

export function addComment(courseId, slideId, text, commentId, isForInstructors) {
	return api.post("/comments/in/" + courseId + "/" + slideId,
		api.createRequestParams({
		'text': text,
		'reply_to': commentId,
		'for_instructors': isForInstructors,
		})
	);
}