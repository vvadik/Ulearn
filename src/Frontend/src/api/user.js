import api from "../api/"

export function getUserProgressInCourse(courseId) {
	return api.get(`user/progress/${ courseId }`);
}

export function updateUserProgressInCourse(courseId, slideId) {
	return api.post((`user/progress/${ courseId }?slideId=${ slideId }`));
}
