let tz = 'Europe/Moscow';

if(process.env.NODE_ENV === 'development') {
	tz = Intl.DateTimeFormat().resolvedOptions().timeZone;
}
export const DEFAULT_TIMEZONE = tz;

export enum TabsType {
	allComments = 'allComments',
	instructorsComments = 'instructorsComments',
}

export enum CourseRoleType {
	courseAdmin = 'courseAdmin',
	instructor = 'instructor',
	student = 'student',
	tester = 'tester',
}

export enum SystemAccessType {
	viewAllProfiles = 'viewAllProfiles',
	viewAllGroupMembers = 'viewAllGroupMembers',
}

export enum CourseAccessType {
	editPinAndRemoveComments = 'editPinAndRemoveComments',
	viewAllStudentsSubmissions = 'viewAllStudentsSubmissions',
	addAndRemoveInstructors = 'addAndRemoveInstructors',
	apiViewCodeReviewStatistics = 'apiViewCodeReviewStatistics',
}

export const AccessType = { ...CourseAccessType, ...SystemAccessType };
export type AccessType = (typeof SystemAccessType) & (typeof CourseAccessType);

export enum ProblemType {
	noEmail, // email не указан
	noName, // нет имени
	emailNotConfirmed, // email не подтвержден
}
