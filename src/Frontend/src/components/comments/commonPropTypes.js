import { shape, objectOf, oneOf, arrayOf, bool, array, number, string } from 'prop-types';

export const userType = shape({
	id: string.isRequired,
	login: string,
	email: string,
	firstName: string,
	lastName: string,
	visibleName: string.isRequired,
	avatarUrl: string,
	gender: string,
});

export const user = shape({
	isAuthenticated: bool,
	user: userType.isRequired,
	systemAccesses: oneOf(['ViewAllProfiles', 'ViewAllGroupMembers']),
});

export const userRoles = shape({
	isSystemAdministrator: bool,
	courseRoles: shape({
		courseId: string,
		role: oneOf(['courseAdmin', 'instructor', 'tester', 'student']),
	}),
	courseAccesses: arrayOf(objectOf(
		shape({
			courseId: string,
			accesses: oneOf(["editPinAndRemoveComments", "ViewAllStudentsSubmissions", "AddAndRemoveInstructors",
				"ViewAllGroupMembers", "ApiViewCodeReviewStatistics"])
		}),
	))
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

