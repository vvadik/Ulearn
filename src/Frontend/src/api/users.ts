import api from "src/api";
import { UsersSearchResponse } from "src/models/users";

export function getCourseInstructors(courseId: string, query?: string, count?: number): Promise<UsersSearchResponse> {
	return api.get(
		'users?courseId=' + courseId + "&courseRole=Instructor" + (query === undefined ? "" : "&query=" + query) + (count === undefined ? "" : "&count=" + count));
}
