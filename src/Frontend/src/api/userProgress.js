import api from "../api/"

export function getUserProgressInCourse(courseId) {
	return api.post(`userProgress/${ courseId }`, api.createRequestParams({}));
}

export function updateUserProgressInCourse(courseId, slideId) {
	return api.post(`userProgress/${ courseId }/visit/${ slideId }`);
}
