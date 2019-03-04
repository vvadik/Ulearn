import React, { Component } from 'react';
import * as debounce from "debounce";
import Thread from "../Thread/Thread";

class CommentsList extends Component {
	constructor(props) {
		super(props);

		this.state = {
			threads: props.comments
			// .map(comment => ({
			// 	comment: {
			// 		...comment,
			// 		replies: comment.replies.map(reply => ({
			// 			comment: reply,
			// 			sending: false,
			// 			// showEditForm: false,
			// 		})),
			// 	},
				// showReplyForm: false,
				// sending: false,
				// showEditForm: false,
			// })),
		};
	}

	render() {
		const { threads } = this.state;
		return (
			<>
				{threads.map((comment) =>
					<Thread
						key={comment.id}
						comment={comment}
						actions={{
							handleLikeClick: this.handleLikeClick,
							handleCorrectAnswerMark: this.handleCorrectAnswerMark,
							handleVisibleMark: this.handleVisibleMark,
							handlePinnedToTopMark: this.handlePinnedToTopMark,
							// handleShowReplyForm: this.handleShowReplyForm,
							// handleShowEditForm: this.handleShowEditForm,
							handleHideEditForm: this.handleHideEditForm,
							handleEditComment: this.handleEditComment,
							handleDeleteComment: this.handleDeleteComment,
							handleAddReplyComment: this.handleAddReplyComment,
							handleSubmitComment: this.handleSubmitComment,
						}}
						getUserSolutionsUrl={this.props.getUserSolutionsUrl}
						user={this.props.user}
						userRoles={this.props.userRoles}
					/>)}
			</>
		)
	}

	findComment(id, threads) {
		for (const thread of threads) {
			if (thread.id === id) {
				return thread;
			}

			for (const reply of thread.replies || []) {
				if (reply.id === id) {
					return reply;
				}
			}
		}
	}

	updateThread(id, reducer) {
		this.setState(() => {
			// const threads = JSON.parse(JSON.stringify(state.threads));
			const threads = [...this.state.threads];
			const thread = threads.find(thread => thread.id === id);

			Object.assign(thread, reducer(thread));

			return threads;
		});
	}

	updateComment(id, reducer) {
		this.setState(() => {
			// const threads = JSON.parse(JSON.stringify(state.threads));
			const threads = [...this.state.threads];
			const comment = this.findComment(id, threads);

			Object.assign(comment, reducer(comment));

			return threads;
		});
	}

	handleLikeClick = (commentId) => {
		this.updateComment(commentId, ({ isLiked, likesCount }) => ({
			likesCount: isLiked ? likesCount - 1 : likesCount + 1,
			isLiked: !isLiked,
		}));

		console.log(`API:toggleLikeClick:#{comment.id}`);
	};

	handleVisibleMark = (commentId) => {

		this.updateComment(commentId, ({ isApproved }) => ({
			isApproved: !isApproved,
		}));

		// this.debouncedSendData(this.handleVisibleMark, commentId, isApproved);
		console.log(`API:toggleVisibleMark:#{comment.id}`);
	};

	handleCorrectAnswerMark = (commentId) => {
		this.updateComment(commentId, ({ isCorrectAnswer }) => ({
			isCorrectAnswer: !isCorrectAnswer,
		}));
		// this.debouncedSendData(this.handleCorrectAnswerMark, commentId, isCorrectAnswer);
		console.log(`API:toggleCorrectAnswerMark:#{comment.id}`);
	};

	handlePinnedToTopMark = (commentId) => {
		this.updateComment(commentId, ({ isPinnedToTop }) => ({
			isPinnedToTop: !isPinnedToTop,
		}));
		// this.debouncedSendData(this.handlePinnedToTopMark, commentId, isPinnedToTop);
		console.log(`API:togglePinnedToTopMark:#{comment.id}`);
	};

	// handleShowReplyForm = (commentId) => {
	// 	this.updateThread(commentId, () => ({
	// 		showReplyForm: true,
	// 	}));
	// };
	//
	// handleShowEditForm = (commentId) => {
	// 	this.updateThread(commentId, () => ({
	// 		showEditForm: true,
	// 	}));
	// };

	// handleHideEditForm = (commentId) => {
	// 	this.updateThread(commentId, () => ({
	// 		showEditForm: false,
	// 	}));
	// };

	handleEditComment = (commentId, text) => {
		// this.updateThread(commentId, () => ({
		// 	showEditForm: false,
		// }));

		this.updateComment(commentId, () => ({
			text,
		}));

		console.log(commentId, `API: updated text:#{comment.id}`);
	};

	handleAddReplyComment = (commentId, text) => {
		// this.updateThread(commentId, () => ({
		// 	showReplyForm: false,
		// }));

		this.updateComment(commentId, () => ({
			text,
		}));

		console.log(`API: added reply to:#{comment.parentCommentId}`);
	};

	handleDeleteComment = (commentId) => {
		const threads = [...this.state.threads];
		const comment = this.findComment(commentId, this.state.threads);
		// const targetThread = threads.find(thread => thread.replies.includes(comment));
		// // console.log(targetThread);
		// const updateThreads = threads.find(thread => thread.id !== targetThread.id);
		// // console.log('target', targetThread);
		// // console.log(updateThreads);
		// const newReplies = targetThread.replies.filter(reply => reply.id !== commentId);
		// console.log(newReplies);
		const newThreads = threads.filter(thread => thread.id !== commentId);

		if (comment.hasOwnProperty('parentCommentId')) {
			// this.updateComment(commentId, () => ({
			// 	replies: newReplies,
			// }));
		} else {
			this.updateThread(commentId, () => newThreads);
		}
	};

	// sendData = (action, value, flag) => {
	// 	return () => action(value, flag);
	// };
}

export default CommentsList;