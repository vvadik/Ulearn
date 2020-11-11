let tz = 'Europe/Moscow';

if(process.env.NODE_ENV === 'development') {
	tz = Intl.DateTimeFormat().resolvedOptions().timeZone;
}
export const DEFAULT_TIMEZONE = tz;

export const TABS = {
	allComments: 'allComments',
	instructorsComments: 'instructorsComments',
};

export const ROLES = {
	systemAdministrator: 'isSystemAdministrator',
	courseAdmin: 'courseAdmin',
	instructor: 'instructor',
	student: 'student',
	tester: 'tester',
};

export const ACCESSES = {
	viewAllProfiles: 'viewAllProfiles',
	viewAllGroupMembers: 'viewAllGroupMembers',
	editPinAndRemoveComments: 'editPinAndRemoveComments',
	viewAllStudentsSubmissions: 'viewAllStudentsSubmissions',
	addAndRemoveInstructors: 'addAndRemoveInstructors',
	apiViewCodeReviewStatistics: 'apiViewCodeReviewStatistics',
};

export const SLIDETYPE = {
	exercise: 'exercise',
	quiz: 'quiz',
	lesson: 'lesson',
	flashcards: 'flashcards',
};

