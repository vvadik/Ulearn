import React, {Component, useContext} from 'react';
import PropTypes from "prop-types";
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import Comment from "../Comment/Comment";
import {CommentContext} from "../CommentsList/CommentsList";
// import {CommentActionHandlers} from "../Comment/commonPropTypes";

import styles from './Thread.less';
import Stub from "../Stub/Stub";


class Thread extends Component {
	constructor(props) {
		super(props);

		this.state = {
			showReplyForm: false,
			sending: false,
		}
	}

	render() {
		const { comment } = this.props;
		const hasReplyAction = !((comment.replies || []).length > 0);


		return (
			this.renderComment(comment, hasReplyAction)
		)
	}

	renderComment(comment, hasReplyAction = false){
		const { getUserSolutionsUrl, user, userRoles } = this.props;
		const { showReplyForm, sending } = this.state;
		console.log('in Thread', this.commentActionHandlers);
		// const { dispatch } = useContext(CommentContext);


		return (
			<Comment
			key={comment.id}
			url={getUserSolutionsUrl(comment.author.id)}
			user={user}
			userRoles={userRoles}
			comment={comment}
			commentActions={this.commentActionHandlers}
			hasReplyAction={hasReplyAction}
			sending={sending}>
			{ (comment.replies || []).map((reply, index, replies) =>
				<div key={reply.id} className={styles.replies}>
					{this.renderComment(reply, index + 1 === replies.length)}
				</div>) }
			{ !comment.parentCommentId && showReplyForm &&
				<div className={styles.replyForm}>
					<CommentSendForm
						author={user}
						autofocus
						sending={sending}
						submitTitle={'Отправить'}
						onSubmit={this.onSubmit} />
				</div> }
			</Comment>
		)
	}

	handleShowReplyForm = () => {
		this.setState({
			showReplyForm: true,
		});
	};

	onSubmit = (text) => {
		this.setState({
			showReplyForm: false,
		});

		this.props.handleAddReplyComment(text);

		//POST: api.comment.addComment()
	};

	commentActionHandlers = {
		handleShowReplyForm: this.handleShowReplyForm,
		handleCorrectAnswerMark: this.props.handleCorrectAnswerMark,
		handlePinnedToTopMark: this.props.handlePinnedToTopMark,
		handleVisibleMark: this.props.handleVisibleMark,
		handleLikeChanged: this.props.handleLikeChanged,
		handleEditComment: this.props.handleEditComment,
		handleDeleteComment: this.props.handleDeleteComment,
	};
}

Thread.propTypes = {
	user: PropTypes.object,
	userRoles: PropTypes.object,
	comment: PropTypes.object,
	getUserSolutionsUrl: PropTypes.func,
	handleVisibleMark: PropTypes.func,
	handlePinnedToTopMark: PropTypes.func,
	handleCorrectAnswerMark: PropTypes.func,
	handleLikeChange: PropTypes.func,
	handleShowEditComment: PropTypes.func,
	handleDeleteComment: PropTypes.func,
	handleAddReplyComment: PropTypes.func,
	// actionHandlers: CommentActionHandlers
};

export default Thread;

