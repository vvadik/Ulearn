import React, { Component } from 'react';
import PropTypes from "prop-types";
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import Comment from "../Comment/Comment";
import {CommentActionHandlers} from "../Comment/commonPropTypes";

class Thread extends Component {
	constructor(props) {
		super(props);

		this.state = {
			comment: props.comment,
			showReplyForm: false,
			sending: false,
		}
	}

	renderComment(comment){
		const { showReplyForm } = this.state;
		const { getUserSolutionsUrl, user, userRoles } = this.props;

		return (
			<Comment
			commentId={comment.id}
			author={comment.author}
			user={user.id}
			userRoles={userRoles}
			text={comment.text}
			renderCommentText={comment.renderedText}
			url={getUserSolutionsUrl(comment.author.id)}
			publishTime={comment.publishTime}
			isApproved={comment.isApproved}
			isPinnedToTop={comment.isPinnedToTop}
			isLiked={comment.isLiked}
			likesCount={comment.likesCount}
			sending={this.state.sending}
			actionHandlers={this.actionHandlers}
			showReplyForm={this.handleShowReplyForm}
			deleteComment={this.deleteComment}
			onEditComment={this.onEditComment}
			onLikeChanged={this.handleLikeChange}
			pinComment={this.handlePinComment}
			markAsCorrectAnswer={this.handleMarkAsCorrectAnswer}
			hideComment={this.handleHideComment}>
			{ comment.replies && comment.replies.length > 0 && comment.replies.map(reply => this.renderComment(reply))}
			{ showReplyForm && <CommentSendForm onSubmit={this.onSubmit}/> }
			</Comment>)

	}
	render() {
		const { comment, } = this.state;
		return this.renderComment(comment);
	}

	handleShowReplyForm = () => {
		this.setState({
			showReplyForm: true,
		});
	};

	handleMarkAsCorrectAnswer = (id, isCorrectAnswer) => {
		this.props.handleMarkAsCorrectAnswer(id, isCorrectAnswer);
		//PATCH: api.comment.changeComment(id, `isCorrectAnswer: ${isCorrectAnswer}`)
	};

	handleHideComment = (id, isApproved) => {
		this.props.handleHideComment(id, isApproved);
		//PATCH: api.comment.changeComment(id, `isApproved: ${isApproved}`)
	};

	handlePinComment = (id, isPinnedToTop) => {
		this.props.handlePinComment(id, isPinnedToTop);
		//PATCH: api.comment.changeComment(id, `isPinnedToTop: ${isPinnedToTop}`)
	};

	handleLikeChange = (id, isLiked) => {
		this.props.handleLikeChange(id, isLiked);

		// if (isLiked) {
			// POST: api.comment.likeComment(id);
		// 	} else {
			//	DELETE: api.comment.dislike(id);
		// }
	};

	onEditComment = (text) => {
		this.props.editComment(text);
		//PATCH: api.comment.changeComment(id, `text: ${text}`)
	};

	deleteComment = (id) => {
		this.props.deleteComment(id);
		//DELETE: api.comment.deleteComment(id);
	};

	onSubmit = (id, text) => {
		this.setState({
			showReplyForm: false,
		});

		this.props.addReplyComment(id, text);

		//POST: api.comment.addComment()
	}
}

Thread.propTypes = {
	comment: PropTypes.object,
	user: PropTypes.object,
	getUserSolutionsUrl: PropTypes.func,
	userRoles: PropTypes.object,
	handleHideComment: PropTypes.func,
	handlePinComment: PropTypes.func,
	handleMarkAsCorrectAnswer: PropTypes.func,
	handleLikeChange: PropTypes.func,
	addReplyComment: PropTypes.func,
	editComment: PropTypes.func,
	deleteComment: PropTypes.func,
	actionHandlers: CommentActionHandlers
};

export default Thread;

