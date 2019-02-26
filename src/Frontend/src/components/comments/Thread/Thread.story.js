import React from 'react';
import { storiesOf } from '@storybook/react';
import Thread from "./Thread";
import PropTypes from "prop-types";
import Comment from "../Comment/Comment";

const user = {
	"id": "11",
	"visibleName": "Louisa",
	"avatarUrl": null,
};

const userRoles = {
	"isSystemAdministrator": true,
	"courseRole": "Student",
	"courseAccesses": ["nothing"],
};

const comment = {
	id: 1,
	text: "Решать эти задачи **можно** прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
	renderedText: "Решать эти задачи <b>можно</b> прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
	author: {
		"id": "11",
		"visibleName": "Louisa",
		"avatarUrl": null,
	},
	publishTime: "2019-01-18T14:12:41.947",
	isApproved: false,
	isPinnedToTop: false,
	isLiked: true,
	likesCount: 10,
	replies: [
		{ id: 2000,
			author: {
				"id": "1",
				"visibleName": "Мария Парадеева",
				"avatarUrl": "https://staff.skbkontur.ru/content/images/default-user-woman.png",
			},
			text: "Я **не согласна**",
			renderedText: "Я <b>не согласна</b>",
			publishTime: "2019-02-18T14:12:41.947",
			isApproved: false,
			isCorrectAnswer: true,
			likesCount: 0,
			parentCommentId: 1999
		}
	],
};

function getUserSolutionUrl(userId){
	return `https://dev.ulearn.me/Analytics/UserSolutions?courseId=BasicProgramming&slideId=90bcb61e-57f0-4baa-8bc9-10c9cfd27f58&userId=${userId}`;
}
storiesOf('Comments/Thread', module)
	.add('комментарий с ответами', () => (
		<Thread comment={comment} user={user} getUserSolutionsUrl={getUserSolutionUrl} userRoles={userRoles} />
	), { viewport: 'desktop' });