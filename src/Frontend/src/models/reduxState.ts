import { SlideUserProgress } from "./userProgress";
import { CourseInfo } from "./course";
import { SubmissionInfo } from "./exercise";
import {CourseAccessType, CourseRoleType, SystemAccessType} from "../consts/accessType";
import { AccountProblemType } from "../consts/accountProblemType";

interface RootState {
	userProgress: UserProgressState;
	courses: CourseState;
	slides: SlidesState;
	account: AccountState;
}

interface UserProgressState {
	loading: boolean;
	progress: { [courseId: string]: { [slideId: string]: SlideUserProgress } };
}

interface CourseState {
	fullCoursesInfo: { [courseId: string]: CourseInfo }
	// TODO не все поля
}

interface SlidesState {
	submissionsByCourses: { [courseId: string]: { [slideId: string]: { [submissionId: number]: SubmissionInfo } } }
	// TODO не все поля
}

interface AccountState {
	accountLoaded: boolean;
	isAuthenticated: boolean;
	isSystemAdministrator: boolean;
	accountProblems: [{ title: string, description: string, problemType: AccountProblemType }];
	systemAccesses: [SystemAccessType];
	roleByCourse: { [courseId: string]: CourseRoleType };
	accessesByCourse: { [courseId: string]: CourseAccessType };
	groupsAsStudent: [GroupAsStudentInfo];
	gender: string | null;
	isHijacked: boolean;
	id: string | null;
	login: string | null;
	firstName: string | null;
	lastName: string | null;
	visibleName: string | null;
	avatarUrl: string | null;
}

export { RootState, UserProgressState, CourseState, AccountState, };
