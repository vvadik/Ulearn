import React, { Component } from 'react';
import PropTypes from "prop-types";
import { userType, userRoles, comment, commentStatus } from "../commonPropTypes";
import Comment from "../Comment/Comment";
import CommentSendForm from "../CommentSendForm/CommentSendForm";

import styles from './Thread.less';

class Thread extends Component {

	render() {
		const {comment} = this.props;
		return this.renderComment(comment);
	}

	renderComment(comment, isLastChild = true) {
		const {user, reply, commentEditing, actions} = this.props;
		const replies = comment.replies || [];
		const isLastCommentInThread = replies.length === 0 && isLastChild;
		const isParentComment = !comment.parentCommentId;

		return (
			<Comment
				key={comment.id}
				comment={comment}
				hasReplyAction={isLastCommentInThread}
				commentEditing={commentEditing}
				// context?
				actions={this.props.actions}
				getUserSolutionsUrl={this.props.getUserSolutionsUrl}
				user={this.props.user}
				userRoles={this.props.userRoles}>
				{replies.map((reply, index) =>
					<div key={reply.id} className={styles.replies}>
						{this.renderComment(reply, index + 1 === replies.length)}
					</div>)}
				{isParentComment && comment.id === reply.commentId &&
				<div className={styles.replyForm}>
					<CommentSendForm
						commentId={comment.id}
						sending={reply.sending}
						author={user}
						submitTitle='Отправить'
						onCancel={() => actions.handleShowReplyForm(null)}
						handleSubmit={actions.handleAddReplyComment} />
				</div>}
			</Comment>
		);
	}
}

Thread.propTypes = {
	user: userType.isRequired,
	userRoles: userRoles.isRequired,
	comment: comment.isRequired,
	reply: commentStatus,
	commentEditing: commentStatus,
	actions: PropTypes.objectOf(PropTypes.func),
	getUserSolutionsUrl: PropTypes.func,
};

export default Thread;

