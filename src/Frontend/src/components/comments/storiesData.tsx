import { UserInfo } from "src/utils/courseRoles";
import { CourseAccessType, CourseRoleType, SystemAccessType, } from "src/consts/accessType";
import { Comment, CommentPolicy, } from "src/models/comments";
import { CommentsApi, FullCommentsApi } from "./utils";
import { ShortUserInfo } from "src/models/users";

interface CommentWithPartialAuthor extends Omit<Partial<Comment>, 'author' | 'replies'> {
	author?: Partial<ShortUserInfo>;
	replies?: CommentWithPartialAuthor[];
}

export const policyCommentsPostModeration: CommentPolicy = {
	areCommentsEnabled: true,
	moderationPolicy: "postmoderation",
	onlyInstructorsCanReply: false,
};

export const getMockedShortUser = (user: Partial<ShortUserInfo>): ShortUserInfo => {
	return {
		id: user.id || '100',
		email: user.email || "mock@email.mocked",
		login: user.login || 'mockedLogin',
		lastName: user.lastName || 'mockedLastName',
		visibleName: user.visibleName || 'mocked visible name',
		firstName: user.firstName || 'mockedFirstname',
		avatarUrl: user.avatarUrl || '',
		gender: user.gender,
	};
};

export const getMockedUser = (user: Partial<UserInfo>): UserInfo => {
	return {
		id: user.id,
		email: user.email,
		login: user.login,
		lastName: user.lastName,
		visibleName: user.visibleName,
		firstName: user.firstName,
		avatarUrl: user.avatarUrl,
		gender: user.gender,
		isSystemAdministrator: user.isSystemAdministrator || false,
		courseRole: user.courseRole || CourseRoleType.student,
		isAuthenticated: user.isAuthenticated || false,
		systemAccesses: user.systemAccesses || [],
		courseAccesses: user.courseAccesses || [],
	};
};

export const getMockedComment = (comment: CommentWithPartialAuthor): Comment => {
	return {
		id: comment.id || 100,
		author: getMockedShortUser(comment.author || {}),
		authorGroups: comment.authorGroups || [],
		text: comment.text || 'mocked text',
		renderedText: comment.renderedText || 'mocked text',
		publishTime: comment.publishTime || '2011-05-12',
		isApproved: comment.isApproved || false,
		isCorrectAnswer: comment.isCorrectAnswer || false,
		isPinnedToTop: comment.isPinnedToTop || false,
		isLiked: comment.isLiked || false,
		likesCount: comment.likesCount || 0,
		replies: comment.replies?.map(getMockedComment) || [],
		parentCommentId: comment.parentCommentId,
		isPassed: comment.isApproved || false,
	};
};

export const fakeFullCommentsApi: FullCommentsApi = {
	getComments: () => Promise.resolve(JSON.parse(JSON.stringify([]))),
	addComment: () => Promise.resolve(getMockedComment({})),
	deleteComment: () => Promise.resolve(),
	updateComment: () => Promise.resolve(getMockedComment({})),
	likeComment: () => Promise.resolve(getMockedComment({})),
	dislikeComment: () => Promise.resolve(getMockedComment({})),
	getCommentPolicy: () => Promise.resolve(policyCommentsPostModeration),
};

export const fakeCommentsApi: CommentsApi = {
	addComment: () => Promise.resolve(getMockedComment({})),
	deleteComment: () => Promise.resolve(),
	updateComment: () => Promise.resolve(getMockedComment({})),
	likeComment: () => Promise.resolve(getMockedComment({})),
	dislikeComment: () => Promise.resolve(getMockedComment({})),
};

export const student: UserInfo = getMockedUser({
	isAuthenticated: true,
	id: "1",
	isSystemAdministrator: false,
	courseRole: CourseRoleType.student,
	visibleName: "Иван Иванов",
	lastName: 'Иванов',
	firstName: 'Иван',
	email: 'student mail',
	login: 'student@urfu.ru',
});

export const unAuthUser: UserInfo = getMockedUser({
	isAuthenticated: false,
});

export const instructor: UserInfo = getMockedUser({
	...student,
	courseRole: CourseRoleType.instructor,
});

export const courseAdmin: UserInfo = getMockedUser({
	...student,
	courseRole: CourseRoleType.courseAdmin,
});

export const sysAdmin: UserInfo = getMockedUser({
	isAuthenticated: true,
	id: "1",
	isSystemAdministrator: true,
	courseRole: CourseRoleType.student,
	visibleName: "Иван Иванов",
	lastName: 'Иванов',
	firstName: 'Иван',
	email: 'admin@ulearn.me',
	login: 'admin',
});

export const avatarUrl = 'https://staff.skbkontur.ru/content/images/default-user-woman.png';

export const accessesToSeeProfiles: SystemAccessType[] = [SystemAccessType.viewAllProfiles];
export const courseAccessesToEditComments: CourseAccessType[] = [CourseAccessType.editPinAndRemoveComments];
export const courseAccessesToViewSubmissions: CourseAccessType[] = [CourseAccessType.viewAllStudentsSubmissions];
