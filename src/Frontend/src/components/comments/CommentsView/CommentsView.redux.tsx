import { connect } from "react-redux";

import {
	addComment, deleteComment,
	dislikeComment,
	getCommentPolicy,
	getComments,
	likeComment,
	updateComment
} from "src/api/comments";

import CommentsView from "./CommentsView";

import { sortComments } from "../utils";

import { RootState } from "src/redux/reducers";
import { Dispatch } from "redux";
import { Comment } from "src/models/comments";


const mapStateToProps = (state: RootState, { courseId, slideId }: { courseId: string, slideId: string, }) => {
	const slideCommentsIds = state.comments.byCourses[courseId]?.bySlides?.[slideId];
	const byIds = state.comments.byIds;
	let comments = undefined;
	let instructorComments = undefined;

	const filterDeletedAndSort = (commentsIds: number[],) => {
		const comments = [];

		for (const commentId of commentsIds) {
			const comment = { ...byIds[commentId] };
			if(!comment.isDeleted) {
				comment.replies = comment.replies.filter(c => !c.isDeleted);
				comments.push(comment);
			}
		}

		sortComments(comments);

		return comments;
	};

	if(slideCommentsIds) {
		if(slideCommentsIds.commentsIds) {
			comments = filterDeletedAndSort(slideCommentsIds.commentsIds);
		}

		if(slideCommentsIds.instructorCommentsIds) {
			instructorComments = filterDeletedAndSort(slideCommentsIds.instructorCommentsIds);
		}
	}

	return {
		deviceType: state.device.deviceType,
		comments,
		instructorComments,
		commentPolicy: state.comments.byCourses[courseId]?.policy,
	};
};

const mapDispatchToProps = (dispatch: Dispatch) => ({
	api: {
		getComments: (courseId: string, slideId: string, forInstructor: boolean) =>
			getComments(courseId, slideId, forInstructor)(dispatch),
		getCommentPolicy: (courseId: string) => getCommentPolicy(courseId)(dispatch),

		addComment: (courseId: string, slideId: string, text: string, forInstructor: boolean,
			parentCommentId?: number
		) =>
			addComment(courseId, slideId, text, parentCommentId, forInstructor)(dispatch).then(c => c.comment),
		deleteComment: (courseId: string, slideId: string, comment: Comment, forInstructor: boolean,) =>
			deleteComment(courseId, slideId, comment, forInstructor)(dispatch),

		likeComment: (commentId: number) => likeComment(commentId)(dispatch),
		dislikeComment: (commentId: number) => dislikeComment(commentId)(dispatch),

		updateComment: (commentId: number,
			updatedFields?: Pick<Partial<Comment>, 'text' | 'isApproved' | 'isCorrectAnswer' | 'isPinnedToTop'>
		) => updateComment(commentId, updatedFields)(dispatch).then(c => c.comment),
	}
});

export default connect(
	mapStateToProps,
	mapDispatchToProps,
)(CommentsView);
