import api from "../api/";

export function getComments(courseId, slideId, forInstructors, offset = 0, count = 200) {
	return api.get("/comments/in/" + courseId + "/" + slideId + "?for_instructors="
	+ encodeURIComponent(forInstructors) + '&offset=' + encodeURIComponent(offset) +
		'&count=' + encodeURIComponent(count));
}

export function addComment(courseId, slideId, text, commentId, isForInstructors) {
	return api.post("/comments/in/" + courseId + "/" + slideId,
		api.createRequestParams({
		'text': text,
		'reply_to': commentId,
		'for_instructors': isForInstructors,
		})
	);
}