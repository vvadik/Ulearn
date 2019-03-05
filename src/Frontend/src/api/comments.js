import api from "../api/";

export const apiRequests = {
	getComments(courseId, slideId, isForInstructors) {
		return api.get("/comments/in/" + courseId + "/" + slideId + "?for_instructors="
			+ encodeURIComponent(isForInstructors));
	},
	addComment(courseId, slideId, text, commentId, isForInstructors) {
		return api.post("/comments/in/" + courseId + "/" + slideId,
			api.createRequestParams({
				'text': text,
				'reply_to': commentId,
				'for_instructors': isForInstructors,
			})
		);
	},
};