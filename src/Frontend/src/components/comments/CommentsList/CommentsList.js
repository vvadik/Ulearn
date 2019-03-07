import React, { Component } from 'react';
import PropTypes from "prop-types";
import { userType, userRoles, comment } from "../commonPropTypes";
//import * as debounce from "debounce";
import Thread from "../Thread/Thread";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import Icon from "@skbkontur/react-icons";

import styles from "./CommentsList.less";

class CommentsList extends Component {
	constructor(props) {
		super(props);

	this.state = {
		threads: [],
		commentEditing: {
			commentId: null,
			sending: false},
		reply: {
			commentId: null,
			sending: false
		},
		sending: false,
		loadingComments: false,
		loadedComments: false,
		status: '',
	};

	//this.debouncedSendData = debounce(this.sendData, 300);
	}

	componentDidMount() {
		const { forInstructors } = this.props;

		this.loadComments(this.courseId, this.slideId, forInstructors);
	};

	loadComments = (courseId, slideId, forInstructors) => {
		const { loadedComments, loadingComments } = this.state;

		if (loadedComments || loadingComments) {
			return;
		}

		this.setState({
			loadingComments: true,
		});

		this.props.commentsApi.getComments(courseId, slideId, forInstructors)
		.then(json => {
			let comments = json.topLevelComments;
			this.setState({
				loadedComments: true,
				threads: comments,
			});
		})
		.catch(() => {
			this.setState({
				status: 'error',
			});
		})
		.finally(() =>
			this.setState({
				loadingComments: false,
			})
		);
	};

	render() {
		const { threads, commentEditing, reply, sending } = this.state;
		return (
			<>
				<CommentSendForm author={this.props.user} handleSubmit={this.handleAddComment} sending={sending} />
				{threads.map((comment) =>
					<Thread
						key={comment.id}
						comment={comment}
						commentEditing={commentEditing}
						reply={reply}
						actions={{
							handleLikeClick: this.handleLikeClick,
							handleCorrectAnswerMark: this.handleCorrectAnswerMark,
							handleApprovedMark: this.handleApprovedMark,
							handlePinnedToTopMark: this.handlePinnedToTopMark,
							handleEditComment: this.handleEditComment,
							handleAddReplyComment: this.handleAddReplyComment,
							handleDeleteComment: this.handleDeleteComment,
							handleShowEditForm: this.handleShowEditForm,
							handleShowReplyForm: this.handleShowReplyForm,
							// handleSubmitComment: this.handleSubmitComment,
						}}
						getUserSolutionsUrl={this.getUserSolutionsUrl}
						user={this.props.user}
						userRoles={this.props.userRoles}
					/>)}
				<button className={styles.sendButton}>
					<Icon name="CommentLite" color="#3072C4"/>
						<span className={styles.sendButtonText}>Оставить комментарий</span>
				</button>
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

	handleAddComment = async (text) => {
		const { commentsApi, courseId, slideId, forInstructors } = this.props;
		const newComment = await commentsApi.addComment(courseId, slideId, text, forInstructors);

		if (!newComment) {
			this.setState({
				sending: true,
			});
		}

		this.setState({
			threads: [newComment, ...this.state.threads],
			sending: false,
		});
	};

	handleLikeClick = (commentId, isLiked) => {
		const { commentsApi } = this.props;

		this.updateComment(commentId, ({ isLiked, likesCount }) => ({
			likesCount: isLiked ? likesCount - 1 : likesCount + 1,
			isLiked: !isLiked,
		}));

		if (isLiked) {
			commentsApi.likeComment(commentId)
				.catch(console.error)
			// this.debouncedSendData(commentsApi.likeComment, commentId);
		}

		commentsApi.dislikeComment(commentId)
			.catch(console.error)
			// this.debouncedSendData(commentsApi.dislikeComment, commentId);
	};

	handleApprovedMark = (commentId, isApproved) => {

		this.updateComment(commentId, ({ isApproved }) => ({
			isApproved: !isApproved,
		}));

		// this.debouncedSendData(commentsApi.updateComment, commentId, isApproved);
		this.props.commentsApi.updateComment(commentId, isApproved)
			.catch(console.error);
	};

	handleCorrectAnswerMark = (commentId, isCorrectAnswer) => {
		this.updateComment(commentId, ({ isCorrectAnswer }) => ({
			isCorrectAnswer: !isCorrectAnswer,
		}));

		// this.debouncedSendData(commentsApi.updateComment, commentId, isCorrectAnswer);
		this.props.commentsApi.updateComment(commentId, isCorrectAnswer)
			.catch(console.error);
	};

	handlePinnedToTopMark = (commentId, isPinnedToTop ) => {
		this.updateComment(commentId, ({ isPinnedToTop }) => ({
			isPinnedToTop: !isPinnedToTop,
		}));

		// this.debouncedSendData(commentsApi.updateComment, commentId, isPinnedToTop);
		this.props.commentsApi.updateComment(commentId, isPinnedToTop)
			.catch(console.error);
	};

	handleShowEditForm = (commentId) => {
		this.setState({
			commentEditing: {...this.state.commentEditing,
				commentId: commentId,
			}
		});
	};

	handleShowReplyForm = (commentId) => {
		console.log(commentId);
		this.setState({
			reply: {...this.state.reply,
				commentId: commentId,
			}
		});
	};

	handleEditComment = (commentId, text) => {
		this.updateComment(commentId, () => ({
			renderedText: text,
		}));

		this.setState({
			commentEditing: {
				commentId: null,
				sending: false,
			}
		});

		// this.debouncedSendData(commentsApi.updateComment, commentId, text);
		this.props.commentsApi.updateComment(commentId, text)
			.then(() =>
				this.setState({
					commentEditing: commentId,
					sending: true,
				})
			)
			.catch(console.error)
			.finally(() =>
				this.setState({
					sending: false,
				})
			)
	};

	handleAddReplyComment = async (commentId, text) => {
		const { commentsApi, courseId, slideId, forInstructors } = this.props;
		const newComment = await commentsApi.addComment(courseId, slideId, text, forInstructors);

		this.updateComment(commentId, () => ({
			text,
		}));

		if (!newComment) {
			this.setState({
				reply: {
					commentId: commentId,
					sending: true,
				}
			});
		}

		this.setState({
			reply: {
				commentId: null,
				sending: false,
			}
		});

		console.log(`API: added reply to:#{comment.parentCommentId}`);
	};

	handleDeleteComment = (commentId) => {
		const threads = [...this.state.threads];
		const comment = this.findComment(commentId, this.state.threads);
		const newThreads = threads.filter(thread => thread.id !== commentId);

		if (comment.parentCommentId) {
		const targetThread = threads.find(thread => thread.replies.includes(comment));
		const updatedThreads = threads.filter(thread => thread.id !== targetThread.id);
		const newReplies = targetThread.replies.filter(reply => reply.id !== commentId);
			this.setState({
				threads: [{...targetThread, replies: newReplies}, ...updatedThreads],
			});
		} else {
			this.setState({
				threads: newThreads,
			});
		}

		Toast.push("Комментарий удалён", {
			label: "Восстановить",
			handler: () => {
				this.setState({
					threads: [...threads],
				});
				Toast.push("Комментарий восстановлен")
			}
		});

		this.props.commentsApi.deleteComment(commentId)
			.catch(console.error);
	};

	getUserSolutionsUrl = (userId) => {
		const { courseId, slideId } = this.props;
		return `${window.location.origin}/Analytics/UserSolutions?courseId=${courseId}&slideId=${slideId}&userId=${userId}`;
	};

	sendData = (action, id, value) => {
		return () => action(id, value);
	};
}

CommentsList.propTypes = {
	user: userType.isRequired,
	userRoles: userRoles.isRequired,
	forInstructors: PropTypes.bool,
	commentsApi: PropTypes.any,
	//comments: PropTypes.arrayOf(comment).isRequired,
	courseId: PropTypes.string.isRequired,
	slideId: PropTypes.string.isRequired,
};

export default CommentsList;