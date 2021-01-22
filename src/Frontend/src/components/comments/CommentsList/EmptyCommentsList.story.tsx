import React from "react";
import CommentsList from "./CommentsList.js";

const user = {
	isAuthenticated: true,
	id: "11",
	visibleName: "Pavel",
	avatarUrl: null,
};

const userIsNotAuthenticated = {
	isAuthenticated: false,
	id: "11",
	visibleName: "Pavel",
	avatarUrl: null,
	systemAccesses: ["viewAllProfiles"],
};

const userIsStudent = {
	isSystemAdministrator: false,
	courseRole: "Student",
	courseAccesses: [],
};

const userIsInstructor = {
	isSystemAdministrator: false,
	courseRole: "instructor",
	courseAccesses: [],
};

function getUserSolutionsUrl(userId: string) {
	return `https://dev.ulearn.me/Analytics/UserSolutions?courseId=BasicProgramming&slideId=90bcb61e-57f0-4baa-8bc9-10c9cfd27f58&userId=${ userId }`;
}


const commentPolicy = {
	areCommentsEnabled: true,
	moderationPolicy: "postmoderation",
	onlyInstructorsCanReply: false,
	status: "ok",
};

const fakeCommentsApi = {
	getComments: () => Promise.resolve({ topLevelComments: [] }),
	getComment: () => Promise.resolve(console.log("API: get comment")),
	addComment: () => Promise.resolve(console.log("API: added comment")),
	deleteComment: () => Promise.resolve(console.log("API: delete comment")),
	updateComment: () => Promise.resolve(console.log("API: update comment")),
	likeComment: () => Promise.resolve(console.log("API: like comment")),
	dislikeComment: () => Promise.resolve(console.log("API: dislike comment")),
};

export default {
	title: "Comments/CommentsList",
};

export const EmptyCommentsListForUnauthorizedUser = () => (
	<CommentsList
		slideType={ "exercise" }
		user={ userIsNotAuthenticated }
		userRoles={ userIsStudent }
		courseId={ "BasicProgramming" }
		slideId={ "90bcb61e-57f0-4baa-8bc9-10c9cfd27f58" }
		commentsApi={ fakeCommentsApi }
		commentPolicy={ commentPolicy }
	/>
);

EmptyCommentsListForUnauthorizedUser.storyName = "empty comments list for unauthorized user";

export const EmptyCommentsListForStudent = () => (
	<CommentsList
		slideType={ "exercise" }
		user={ user }
		userRoles={ userIsStudent }
		courseId={ "BasicProgramming" }
		slideId={ "90bcb61e-57f0-4baa-8bc9-10c9cfd27f58" }
		commentsApi={ fakeCommentsApi }
		commentPolicy={ commentPolicy }
	/>
);

EmptyCommentsListForStudent.storyName = "empty comments list for student";

export const EmptyCommentsListForInstructor = () => (
	<CommentsList
		slideType={ "exercise" }
		getUserSolutionsUrl={ getUserSolutionsUrl(user.id) }
		user={ user }
		userRoles={ userIsInstructor }
		courseId={ "BasicProgramming" }
		slideId={ "90bcb61e-57f0-4baa-8bc9-10c9cfd27f58" }
		commentsApi={ fakeCommentsApi }
		commentPolicy={ commentPolicy }
	/>
);

EmptyCommentsListForInstructor.storyName = "empty comments list for instructor";
