import { SlideUserProgress } from "./userProgress";
import { CourseInfo } from "./course";
import { SubmissionInfo } from "./exercise";
import { SystemAccessType, CourseAccessType, CourseRoleType, ProblemType, } from "../consts/general";

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
	accountProblems: [{ title: string, description: string, problemType: ProblemType }];
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
