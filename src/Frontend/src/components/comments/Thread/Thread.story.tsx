import React from "react";
import Thread from "./Thread";
import { CommentPolicy, ResponseStatus, Comment as CommentType } from "src/models/comments";

const actions = {
	handleLikeClick: () => console.log("likeComment"),
	handleCorrectAnswerMark: () => console.log("correctMark"),
	handleApprovedMark: () => console.log("approvedMark"),
	handlePinnedToTopMark: () => console.log("pinnedToTopMark"),
	handleEditComment: () => console.log("editComment"),
	handleAddReplyComment: () => console.log("addReplyComment"),
	handleDeleteComment: () => console.log("deleteComment"),
	handleShowEditForm: () => console.log("showEditForm"),
	handleShowReplyForm: () => console.log("showReplyForm"),
};

const user = {
	is_authenticated: true,
	id: "11",
	visibleName: "Pavel",
	avatarUrl: null,
};

const userRoles = {
	isSystemAdministrator: true,
	courseRole: "Student",
	courseAccesses: [
		{
			accesses: null,
		},
	],
};

const commentEditing = {
	commentId: null,
	sending: false,
};

const reply = {
	commentId: null,
	sending: false,
};

const replies = [];

for (let i = 0; i < 5; i++) {
	replies.push({
		id: 2 + i,
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
	});
}

const commentPolicy : CommentPolicy= {
	areCommentsEnabled: true,
	moderationPolicy: "postmoderation",
	onlyInstructorsCanReply: false,
};

const comment :CommentType= {
	id: '1',
	text:
		"Решать эти задачи **можно** прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
	renderedText:
		"Решать эти задачи <b>можно</b> прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
	author: {
		id: "11",
		visibleName: "Louisa",
		avatarUrl: null,
		firstName:"Louisa",
		gender:null,
		lastName:'',
	},
	publishTime: "2019-01-18T14:12:41.947",
	isApproved: false,
	isPinnedToTop: false,
	isLiked: true,
	likesCount: 10,
	replies: replies,
};

function getUserSolutionUrl(userId: string) {
	return `https://dev.ulearn.me/Analytics/UserSolutions?courseId=BasicProgramming&slideId=90bcb61e-57f0-4baa-8bc9-10c9cfd27f58&userId=${ userId }`;
}

export default {
	title: "Comments/Thread",
};

export const CommentWithReplies = () => (
	<Thread
		commentPolicy={ commentPolicy }
		comment={ comment }
		user={ user }
		getUserSolutionsUrl={ getUserSolutionUrl }
		userRoles={ userRoles }
		commentEditing={ commentEditing }
		reply={ reply }
		actions={ actions }
	/>
);

CommentWithReplies.storyName = "comment with replies";
