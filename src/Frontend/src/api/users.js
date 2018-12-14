import api from "../api/"

export function getCourseInstructors(courseId) {
	return api.get('/users/' + courseId + '/instructors')
		.then(response => response.json());
}