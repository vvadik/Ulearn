import React, { Component } from "react";
import PropTypes from "prop-types";
import { userType, userRoles, comment, commentStatus } from "../commonPropTypes";
import { TransitionGroup, CSSTransition } from "react-transition-group";
import Comment from "../Comment/Comment";
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import { Mobile, NotMobile } from "../../../utils/responsive";

import styles from "./Thread.less";

class Thread extends Component {

	render() {
		const {comment} = this.props;
		return this.renderComment(comment);
	}

	renderComment(comment, isLastChild = true) {
		const {user, userRoles, reply, commentEditing, actions, getUserSolutionsUrl, slideType} = this.props;
		const replies = comment.replies || [];
		const isLastCommentInThread = replies.length === 0 && isLastChild;
		const isParentComment = !comment.parentCommentId;

		return (
			<>
				<Comment
					key={comment.id}
					comment={comment}
					hasReplyAction={isLastCommentInThread}
					commentEditing={commentEditing}
					actions={actions}
					getUserSolutionsUrl={getUserSolutionsUrl}
					slideType={slideType}
					user={user}
					userRoles={userRoles}>
					<NotMobile>
						{this.renderReplies(comment)}
					</NotMobile>
					{(isParentComment && comment.id === this.props.reply.commentId) &&
					<div className={styles.replyForm}>
						<CommentSendForm
							commentId={comment.id}
							sending={reply.sending}
							author={user}
							submitTitle="Отправить"
							onCancel={() => actions.handleShowReplyForm(null)}
							handleSubmit={actions.handleAddReplyComment} />
					</div>}
				</Comment>
				<Mobile>
					{this.renderReplies(comment)}
				</Mobile>
			</>
		)
	}

	renderReplies(comment) {
		const replies = comment.replies || [];
		return (
			<div className={styles.repliesWrapper}>
				<TransitionGroup enter={this.props.animation}>
					{replies.map((reply, index) =>
						<CSSTransition
							key={reply.id}
							mountOnEnter
							unmountOnExit
							in={this.props.animation}
							classNames={{
								enter: styles.enter,
								exit: styles.exit,
								enterActive: styles.enterActive,
								exitActive: styles.exitActive,
							}}
							timeout={500}>
							<div key={reply.id} className={styles.reply}>
								{this.renderComment(reply, index + 1 === replies.length)}
							</div>
						</CSSTransition>
					)}
				</TransitionGroup>
			</div>
		)
	}
}

Thread.propTypes = {
	user: userType.isRequired,
	userRoles: userRoles.isRequired,
	slideType: PropTypes.string,
	comment: comment.isRequired,
	reply: commentStatus,
	commentEditing: commentStatus,
	actions: PropTypes.objectOf(PropTypes.func),
	animation: PropTypes.bool,
	getUserSolutionsUrl: PropTypes.func,
};

export default Thread;

