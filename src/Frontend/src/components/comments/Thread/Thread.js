import React, { Component } from 'react';
import Comment from "../Comment/Comment";
import CommentSendForm from "../CommentSendForm/CommentSendForm";

import styles from './Thread.less';

class Thread extends Component {
	state = {
		showReplyForm: false,
		sending: false,
	};

	render() {
		const { comment } = this.props;

		return this.renderComment(comment);
	}

	renderComment(comment, isLastChild = true) {
		const { user } = this.props;
		const replies = comment.replies || [];

		const isLastCommentInThread = replies.length === 0 && isLastChild;
		const isParentComment = !comment.parentCommentId;

		return (
			<Comment
				key={comment.id}
				comment={comment}
				hasReplyAction={isLastCommentInThread}
				handleShowReplyForm={this.handleShowReplyForm}
				showEditForm={this.state.showEditForm}
				// context?
				actions={this.props.actions}
				getUserSolutionsUrl={this.props.getUserSolutionsUrl}
				user={this.props.user}
				userRoles={this.props.userRoles} >
			{ replies.map((reply, index) =>
				<div key={reply.id} className={styles.replies}>
					{this.renderComment(reply, index + 1 === replies.length)}
				</div>) }
			{ isParentComment && this.state.showReplyForm &&
				<div className={styles.replyForm}>
					<CommentSendForm
						commentId={comment.id}
						author={user}
						sending={comment.sending}
						submitTitle='Отправить'
						handleShowForm={this.handleShowReplyForm}
						handleSubmit={this.props.actions.handleAddReplyComment} />
				</div> }
			</Comment>
		);
	}

		handleShowReplyForm = (flag) => {
			this.setState({
				showReplyForm: flag,
			});
		};
}

export default Thread;

