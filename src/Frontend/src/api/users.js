import api from "../api/"

export function getCourseInstructors(courseId) {
	return api.get('users?course_id=' + courseId + "&course_role=Instructor");
}