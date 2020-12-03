import React from "react";
import { Review, ReviewProps } from "./Review";

import { ReviewInfo, SolutionRunStatus, } from "src/models/exercise";
import type { Story } from "@storybook/react";
import { SubmissionColor } from "../ExerciseUtils";

const addingTime = "2020-12-03T20:03:29.9725057+05:00";

const author = {
	avatarUrl: "",
	firstName: "Name",
	gender: null,
	id: "0",
	lastName: "LastName",
	visibleName: "Name LastName",
	login: undefined,
	email: undefined,
};

const comment = {
	id: 0,
	text: "text **bold** __italic__ ```code```",
	renderedText: "text <b>bold</b> <i>italic</i> <pre>code</pre>",
	publishTime: addingTime,
	author: author
};

const review = {
	id: 0,
	author: null,
	startLine: 10,
	startPosition: 5,
	finishLine: 10,
	finishPosition: 5,
	comment: "text **bold** __italic__ ```code```",
	renderedComment: "text <b>bold</b> <i>italic</i> <pre>code</pre>",
	addingTime: null,
	comments: [comment, { ...comment, id: 1 }]
};

const teacherReview = {
	...review,
	author,
	addingTime,
};

const reviews = [
	review,
	{ ...teacherReview, id: 1 },
	{ ...teacherReview, id: 2 },
];

const props = {
	reviews,
	selectedReviewId: -1,
	userId: "-1",
	onSelectComment: (e: React.MouseEvent | React.FocusEvent, id: number,) => {
	},
	addReviewComment: (reviewId: number, comment: string) => {
	},
	deleteReviewComment: (reviewId: number, commentId: number) => {
	},
	getReviewAnchorTop: (review: ReviewInfo) => review.startLine * 15,
};

const Template: Story<ReviewProps> = (args: ReviewProps) =>
	<div style={ { width: '260px', position: 'relative', display: 'flex', } }>
		<Review { ...args }/>
	</div>;

export const NothingSelected = Template.bind({});
NothingSelected.args = {
	...props,
};

export const FirstSelected = Template.bind({});
FirstSelected.args = {
	...props,
	selectedReviewId: 0,
};

export const SecondSelected = Template.bind({});
SecondSelected.args = {
	...props,
	selectedReviewId: 1,
};

export const ThirdSelected = Template.bind({});
ThirdSelected.args = {
	...props,
	selectedReviewId: 2,
};

export const UserCanDeleteComment = Template.bind({});
UserCanDeleteComment.args = {
	...props,
	userId: author.id,
	selectedReviewId: 1,
};

export const SpaceAmongReviews = Template.bind({});
SpaceAmongReviews.args = {
	...props,
	reviews: [
		{ ...review, startLine: 0, finishLine: 0, },
		{ ...review, startLine: 20, finishLine: 20, },
	],
};

export const LinesCompare = Template.bind({});
LinesCompare.args = {
	...props,
	reviews: [
		{ ...review, startLine: 0, finishLine: 0, },
		{ ...review, startLine: 1, finishLine: 2, },
		{ ...review, startLine: 2, finishLine: 10, },
	],
};

export default {
	title: 'Exercise/Review',
	component: Review,
};
