import React from "react";
import { withViewport } from "@storybook/addon-viewport";
import CommentsView from "./CommentsView";

const comments = [
	{
		id: 1999,
		text:
			"Решать эти задачи **можно** прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
		renderedText:
			"Решать эти задачи <b>можно</b> прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
		author: {
			id: "11",
			visibleName: "Louisa",
			avatarUrl: null,
		},
		publishTime: "2019-01-18T14:12:41.947",
		isApproved: false,
		isPinnedToTop: false,
		isLiked: true,
		likesCount: 10,
		replies: [
			{
				id: 2000,
				author: {
					id: "10",
					visibleName: "Maria",
					avatarUrl:
						"https://staff.skbkontur.ru/content/images/default-user-woman.png",
				},
				text: "Я **не согласна**",
				replies: [],
				renderedText: "Я <b>не согласна</b>",
				publishTime: "2019-02-18T14:12:41.947",
				isApproved: true,
				isCorrectAnswer: false,
				likesCount: 0,
				isLiked: false,
				parentCommentId: 1999,
			},
			{
				id: 2001,
				author: {
					id: "11",
					visibleName: "Kate",
					avatarUrl:
						"https://staff.skbkontur.ru/content/images/default-user-woman.png",
				},
				text: "Я **согласна**",
				replies: [],
				renderedText: "Я <b>согласна</b>",
				publishTime: "2019-02-18T14:12:41.947",
				isApproved: false,
				isCorrectAnswer: true,
				likesCount: 5,
				isLiked: true,
				parentCommentId: 1999,
			},
		],
	},
	{
		id: 2002,
		text:
			"Решать эти задачи **можно** прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
		renderedText:
			"Решать эти задачи <b>можно</b> прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
		author: {
			id: "13",
			visibleName: "Henry",
			avatarUrl: null,
		},
		publishTime: "2019-01-18T14:12:41.947",
		isApproved: false,
		isPinnedToTop: false,
		isLiked: true,
		likesCount: 8,
		replies: [],
	},
];

const user = {
	isAuthenticated: true,
	id: "11",
	visibleName: "Pavel",
	avatarUrl: null,
	systemAccesses: ["viewAllProfiles"],
};

const userRoles = {
	isSystemAdministrator: true,
	courseRole: "Instructor",
	courseAccesses: ["editPinAndRemoveComments"],
};

const fakeCommentsApi = {
	getComments: () => Promise.resolve({ topLevelComments: comments }),
	addComment: () => Promise.resolve(console.log("API: added comment")),
	deleteComment: () => Promise.resolve(console.log("API: delete comment")),
	updateComment: () => Promise.resolve(console.log("API: update comment")),
	likeComment: () => Promise.resolve(console.log("API: like comment")),
	dislikeComment: () => Promise.resolve(console.log("API: dislike comment")),
};

export default {
	title: "Comments/CommentsView",
	decorators: [withViewport(), withViewport(), withViewport()],
};

export const Desktop = () => (
	<CommentsView
		slideType={"exercise"}
		user={user}
		userRoles={userRoles}
		slideId={"90bcb61e-57f0-4baa-8bc9-10c9cfd27f58"}
		courseId={"BasicProgramming"}
		commentsApi={fakeCommentsApi}
	/>
);

Desktop.storyName = "desktop";
Desktop.parameters = { viewport: "desktop" };

export const Tablet = () => (
	<CommentsView
		slideType={"exercise"}
		user={user}
		userRoles={userRoles}
		slideId={"90bcb61e-57f0-4baa-8bc9-10c9cfd27f58"}
		courseId={"BasicProgramming"}
		commentsApi={fakeCommentsApi}
	/>
);

Tablet.storyName = "tablet";
Tablet.parameters = { viewport: "tablet" };

export const Mobile = () => (
	<CommentsView
		slideType={"exercise"}
		user={user}
		userRoles={userRoles}
		slideId={"90bcb61e-57f0-4baa-8bc9-10c9cfd27f58"}
		courseId={"BasicProgramming"}
		commentsApi={fakeCommentsApi}
	/>
);

Mobile.storyName = "mobile";
Mobile.parameters = { viewport: "mobile" };
