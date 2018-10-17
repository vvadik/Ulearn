import api from "../api/"

export function getCourseGroups(courseId) {
	return api.get("groups/in/" + courseId)
		.then(response => response.json())
}