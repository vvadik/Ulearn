import React from "react";
import CommentsView, { Props } from "./CommentsView";
import { Comment, CommentPolicy } from "src/models/comments";
import { SlideType } from "src/models/slide";
import { DeviceType } from "src/consts/deviceType";
import {
	accessesToSeeProfiles, courseAccessesToEditComments, courseAccessesToViewSubmissions, courseAdmin,
	fakeFullCommentsApi,
	getMockedComment,
	instructor,
	student,
	sysAdmin,
	unAuthUser
} from "../storiesData";
import type { Story } from "@storybook/react";

const comments: Comment[] = [
	{
		id: 1999,
		text:
			"Решать эти задачи **можно** прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
		renderedText:
			"Решать эти задачи <b>можно</b> прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
		author: {
			id: "11",
			visibleName: "Louisa",
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
					id: "1",
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
		},
		publishTime: "2019-01-18T14:12:41.947",
		isApproved: true,
		isPinnedToTop: false,
		isLiked: true,
		likesCount: 8,
		replies: [],
	},
].map(getMockedComment);

const commentPolicy: CommentPolicy = {
	areCommentsEnabled: false,
	moderationPolicy: "postmoderation",
	onlyInstructorsCanReply: false,
};

export default {
	title: "Comments/CommentsView",
};

const Template: Story<Props> = args => <CommentsView
	{ ...args }
	commentsCount={ comments.length }
	instructorCommentsCount={ 0 }
	instructorComments={ [] }
	deviceType={ DeviceType.desktop }
	isSlideReady={ true }
	slideType={ SlideType.Exercise }
	comments={ comments }
	slideId={ "90bcb61e-57f0-4baa-8bc9-10c9cfd27f58" }
	courseId={ "BasicProgramming" }
	api={ fakeFullCommentsApi }
	commentPolicy={ commentPolicy }
/>;

export const UnauthorizedUser = Template.bind({});
UnauthorizedUser.args = {
	user: unAuthUser,
};

export const UserIsStudent = Template.bind({});
UserIsStudent.args = {
	user: student,
};

export const UserIsInstructor = Template.bind({});
UserIsInstructor.args = {
	user: instructor,
};

export const UserIsInstructorWithAccessesToSeeProfiles = Template.bind({});
UserIsInstructorWithAccessesToSeeProfiles.args = {
	user: { ...instructor, systemAccesses: accessesToSeeProfiles },
};

export const UserIsInstructorWithModerateAccesses = Template.bind({});
UserIsInstructorWithModerateAccesses.args = {
	user: { ...instructor, courseAccesses: courseAccessesToEditComments },
};

export const UserIsInstructorWithAccessesViewSubmission = Template.bind({});
UserIsInstructorWithAccessesViewSubmission.args = {
	user: { ...instructor, courseAccesses: courseAccessesToViewSubmissions },
};

export const UserIsCourseAdmin = Template.bind({});
UserIsCourseAdmin.args = {
	user: courseAdmin,
};

export const UserIsSysadmin = Template.bind({});
UserIsSysadmin.args = {
	user: sysAdmin,
};


