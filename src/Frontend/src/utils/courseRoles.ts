import { CourseAccessType, CourseRoleType, SystemAccessType } from "src/consts/accessType";
import { ShortUserInfo } from "../models/users";

function isCourseAdmin(userRoles: UserRoles): boolean {
	return userRoles.isSystemAdministrator ||
		userRoles.courseRole === CourseRoleType.courseAdmin;
}

function isInstructor(userRoles: UserRoles): boolean {
	return isCourseAdmin(userRoles) ||
		userRoles.courseRole === CourseRoleType.instructor;
}

interface UserRoles {
	isSystemAdministrator: boolean;
	courseRole: CourseRoleType;
}

interface UserInfo extends ShortUserInfo, UserRoles {
	isAuthenticated: boolean;
	systemAccesses: SystemAccessType[];
	courseAccesses: CourseAccessType[];
}

export { isCourseAdmin, isInstructor, UserRoles, UserInfo, };
