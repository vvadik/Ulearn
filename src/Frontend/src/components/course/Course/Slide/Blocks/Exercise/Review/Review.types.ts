import { InstructorReviewInfoWithAnchor } from "../../../InstructorReview/InstructorReview.types";
import React from "react";
import { UserInfo } from "src/utils/courseRoles";

export interface CommentReplies {
	[id: number]: string;
}

export interface RenderedReview {
	margin: number;
	prevMargin?: number;
	review: InstructorReviewInfoWithAnchor;
	ref: React.RefObject<HTMLLIElement>;
}

export interface ReviewState {
	renderedReviews: RenderedReview[];
	replies: CommentReplies;

	editingParentReviewId?: number;
	editingReviewId?: number;
	editingCommentValue?: string;
}

export interface ReviewProps {
	reviews: InstructorReviewInfoWithAnchor[];
	selectedReviewId: number;
	user?: UserInfo;

	isReviewCanBeAdded: (reviewText: string) => boolean;
	onReviewClick: (e: React.MouseEvent | React.FocusEvent, id: number,) => void;
	addReviewComment: (reviewId: number, comment: string) => void;
	deleteReviewOrComment: (id: number, reviewId?: number) => void;
	editReviewOrComment: (text: string, id: number, reviewId?: number) => void;

	backgroundColor?: 'orange' | 'gray';

	toggleReviewFavourite?: (id: number,) => void;
}
