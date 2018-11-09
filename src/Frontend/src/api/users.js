import api from "../api/"

export function getUsersCourse(courseId) {
	return api.get('/users/' + courseId + '/instructors')
		.then(response => response.json());
}