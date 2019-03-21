import { shape, oneOf, arrayOf, bool, array, number, string } from 'prop-types';

export const userType = shape({
	id: string.isRequired,
	visibleName: string.isRequired,
	login: string,
	email: string,
	firstName: string,
	lastName: string,
	avatarUrl: string,
	gender: string,
});

export const user = shape({
	isAuthenticated: bool.isRequired,
	id: string.isRequired,
	visibleName: string.isRequired,
	avatarUrl: string,
	systemAccesses: array,
});

export const userRoles = shape({
	isSystemAdministrator: bool,
	courseRoles: oneOf(['courseAdmin', 'instructor', 'tester', 'student']),
	courseAccesses: arrayOf(oneOf(["editPinAndRemoveComments", "viewAllStudentsSubmissions", "addAndRemoveInstructors",
		"viewAllGroupMembers", "apiViewCodeReviewStatistics"])),
});

export const comment = shape({
	id: number.isRequired,
	author: userType.isRequired,
	text: string,
	renderedText: string,
	publishTime: string,
	isApproved: bool,
	isCorrectAnswer: bool,
	isPinnedToTop: bool,
	likesCount: number,
	replies: array,
	parentCommentId: number,
	courseId: string,
	slideId: string,
});

export const commentStatus = shape({
	commentId: number,
	sending: bool,
});

