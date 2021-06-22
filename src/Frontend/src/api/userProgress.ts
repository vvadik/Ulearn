import api from "./index";
import { UsersProgressResponse } from "src/models/userProgress";

export function getUserProgressInCourse(courseId: string): Promise<UsersProgressResponse> {
	return api.post(`user-progress/${ courseId }`, api.createRequestParams({}));
}

export function updateUserProgressInCourse(courseId: string, slideId: string): Promise<UsersProgressResponse> {
	return api.post(`user-progress/${ courseId }/visit/${ slideId }`);
}
export function skipExercise(courseId: string, slideId: string): Promise<Response> {
	return api.put(`user-progress/${ courseId }/exercise/${ slideId }/skip`);
}
