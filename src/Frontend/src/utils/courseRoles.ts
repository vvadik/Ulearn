import { CourseRoleType } from "src/consts/general";

function isCourseAdmin(userRoles: UserRoles): boolean {
	return userRoles.isSystemAdministrator ||
		userRoles.courseRole === CourseRoleType.courseAdmin;
}

function isInstructor(userRoles: UserRoles): boolean {
	return isCourseAdmin(userRoles) ||
		userRoles.courseRole === CourseRoleType.instructor;
}

interface UserRoles {
	isSystemAdministrator: boolean,
	courseRole: CourseRoleType,
}

export { isCourseAdmin, isInstructor, UserRoles };
