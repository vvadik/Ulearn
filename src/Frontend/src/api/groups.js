import api from "../api/"

export function getCourseGroups__(courseId) {
	return dispatch => {
		return api.get("groups/in/" + courseId)
			.then(response => response.json())
			.then(json => {
				console.log(json);
			})
	}
}

export function getCourseGroups(courseId) {
	return api.get("groups/in/" + courseId)
		.then(response => response.json())
}