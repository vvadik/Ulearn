import React, { Component } from "react";
import PropTypes from "prop-types";
import { userType, userRoles, commentPolicy } from "../commonPropTypes";
import { TransitionGroup, CSSTransition } from "react-transition-group";
import * as debounce from "debounce";
import Icon from "@skbkontur/react-icons";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import Thread from "../Thread/Thread";
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import Error404 from "../../common/Error/Error404";
import Stub from "../Stub/Stub";
import scrollToView from "../../../utils/scrollToView";

import styles from "./CommentsList.less";

class CommentsList extends Component {
	constructor(props) {
		super(props);

		this.state = {
			newCommentId: 1,
			saveCommentLikeStatus: null,
			threads: [],
			commentEditing: {
				commentId: null,
				sending: false
			},
			reply: {
				commentId: null,
				sending: false
			},
			sending: false,
			loadingComments: false,
			status: "",
			animation: false,
		};

		this.lastThreadRef = React.createRef();

		this.debouncedSendData = debounce(this.sendData, 300);
	}

	componentDidMount() {
		const {courseId, slideId, forInstructors} = this.props;

		this.loadComments(courseId, slideId, forInstructors);
	};

	loadComments = (courseId, slideId, forInstructors) => {
		if (this.state.loadingComments) {
			return;
		}

		this.setState({
			loadingComments: true,
		});

		this.props.commentsApi.getComments(courseId, slideId, forInstructors)
			.then(json => {
				const comments = json.topLevelComments;
				this.setState({
					threads: comments,
					loadingComments: false,
				});
			})
			.catch(() => {
				this.setState({
					status: "error",
				});
			})
			.finally(() => {
				this.setState({
					loadingComments: false,
				})
			});
	};

	render() {
		const {threads, loadingComments} = this.state;
		const {user, courseId, slideId, commentPolicy} = this.props;

		if (this.state.status === "error") {
			return <Error404 />;
		}

		if (!courseId || !slideId) {
			return null;
		}

		return (
			<>
				{loadingComments ? <div className={styles.spacer}><Loader type="big" active={loadingComments} /></div> :
					<>
						{!user.id && <Stub hasThreads={threads.length > 0} courseId={courseId} slideId={slideId} />}
						{(user.id && !commentPolicy.areCommentsEnabled) && this.renderMessageIfCommentsDisabled()}
						{this.renderSendForm()}
						{this.renderThreads()}
					</>}
				{(commentPolicy.areCommentsEnabled && user.id && threads.length > 7) &&
				<button className={styles.sendButton} onClick={() => this.handleScroll(this.props.headerRef)}>
					<Icon name="CommentLite" color="#3072C4" />
					<span className={styles.sendButtonText}>Оставить комментарий</span>
				</button>}
			</>
		)
	}

	renderMessageIfCommentsDisabled() {
		return (
			<div className={styles.textForDisabledComments}>
				В данный момент комментарии выключены.
			</div>
			)
	};

	renderSendForm() {
		const {sending, status, newCommentId} = this.state;
		const {user, forInstructors, commentPolicy} = this.props;

		return (
			user.id && (commentPolicy.areCommentsEnabled || commentPolicy.onlyInstructorsCanReply) &&
				<CommentSendForm
					isForInstructors={forInstructors}
					commentId={newCommentId}
					author={user}
					handleSubmit={this.handleAddComment}
					sending={sending}
					sendStatus={status} />
		)
	};

	renderThreads() {
		const {threads, commentEditing, reply, animation} = this.state;
		const {user, userRoles, slideType, commentPolicy} = this.props;
		const transitionStyles = {
			enter: styles.enter,
			exit: styles.exit,
			enterActive: styles.enterActive,
			exitActive: styles.exitActive,
		};

		const duration = {
			enter: 1000,
			exit: 500,
		};

		const actions = {
			handleLikeClick: this.handleLikeClick,
			handleCorrectAnswerMark: this.handleCorrectAnswerMark,
			handleApprovedMark: this.handleApprovedMark,
			handlePinnedToTopMark: this.handlePinnedToTopMark,
			handleEditComment: this.handleEditComment,
			handleAddReplyComment: this.handleAddReplyComment,
			handleDeleteComment: this.handleDeleteComment,
			handleShowEditForm: this.handleShowEditForm,
			handleShowReplyForm: this.handleShowReplyForm,
		};

		return (
			<TransitionGroup enter={animation}>
				{threads
				.map((comment) =>
					<CSSTransition
						key={comment.id}
						appear
						in={animation}
						mountOnEnter
						unmountOnExit
						classNames={transitionStyles}
						timeout={duration}>
						<section className={styles.thread} key={comment.id} ref={this.lastThreadRef}>
							<Thread
								user={user}
								userRoles={userRoles}
								slideType={slideType}
								animation={animation}
								comment={comment}
								commentPolicy={commentPolicy}
								commentEditing={commentEditing}
								reply={reply}
								actions={actions}
								getUserSolutionsUrl={this.getUserSolutionsUrl} />
						</section>
					</CSSTransition>)}
			</TransitionGroup>
		)
	}

	findCommentPosition(id, threads) {
		for (let i = 0; i < threads.length; i++) {
			const thread = threads[i];
			if (thread.id === id) {
				return [threads, i];
			}

			if (!thread.replies) continue;

			for (let j = 0; j < thread.replies.length; j++) {
				const reply = thread.replies[j];
				if (reply.id === id) {
					return [thread.replies, j];
				}
			}
		}
	}

	findComment(id, threads) {
		const [parentArray, index] = this.findCommentPosition(id, threads);

		return parentArray[index];
	}

	deleteComment(id, threads) {
		const [parentArray, index] = this.findCommentPosition(id, threads);
		const comment = parentArray[index];
		parentArray.splice(index, 1);

		function restoreComment() {
			parentArray.splice(index, 0, comment);
		}

		return restoreComment;
	}

	compareThreads(a, b) {
		if (a.isPinnedToTop && !b.isPinnedToTop) {
			return -1;
		}

		if (!a.isPinnedToTop && b.isPinnedToTop) {
			return 1;
		}

		if (a.publishTime > b.publishTime ) {
			return -1;
		}

		if (a.publishTime < b.publishTime ) {
			return 1;
		}

		return 0;
	}

	updateComment(id, reducer) {
		const threads = [...this.state.threads];
		const comment = this.findComment(id, threads);

		Object.assign(comment, reducer(comment));

		if (!comment.parentCommentId) {
			threads.sort(this.compareThreads)
		}

		this.setState({ threads });
	}

	handleAddComment = async (commentId, text) => {
		const {commentsApi, courseId, slideId, forInstructors, handleInstructorsCommentCount} = this.props;
		const threads = [...this.state.threads];

		this.setState({
			sending: true,
			animation: true,
		});

		try {
			const newComment = await commentsApi.addComment(courseId, slideId, text, null, forInstructors);
			const pinnedComments = threads.filter(comment => comment.isPinnedToTop);
			const filteredComments = threads.filter(comment => !comment.isPinnedToTop)
			.sort((a, b) => new Date(b.publishTime) - new Date(a.publishTime));

			this.setState({
				threads: [
					...pinnedComments,
					newComment,
					...filteredComments,
				],
				newCommentId: this.state.newCommentId + 1,
			});

			this.handleScroll(this.lastThreadRef);

			if (forInstructors) {
				handleInstructorsCommentCount("add");
			}
		}
		catch (e) {
			Toast.push("Не удалось добавить комментарий. Попробуйте снова.");
			this.setState({
				newCommentId: this.state.newCommentId,
			});
			console.error(e);
		}
		finally {
			this.setState({
				sending: false,
			})
		}
	};

	handleLikeClick = (commentId, isLiked) => {
		const {commentsApi} = this.props;

		this.updateComment(commentId, ({ likesCount }) => ({
			likesCount: isLiked ? likesCount - 1 : likesCount + 1,
			isLiked: !isLiked,
		}));

		this.debouncedSendData(isLiked ? commentsApi.dislikeComment : commentsApi.likeComment, commentId);
	};

	handleApprovedMark = (commentId, isApproved) => {
		this.updateComment(commentId, () => ({
			isApproved: !isApproved,
		}));

		this.debouncedSendData(this.props.commentsApi.updateComment, commentId, {"isApproved": !isApproved});
	};

	handleCorrectAnswerMark = (commentId, isCorrectAnswer) => {
		this.updateComment(commentId, () => ({
			isCorrectAnswer: !isCorrectAnswer,
		}));

		this.debouncedSendData(this.props.commentsApi.updateComment, commentId, {"isCorrectAnswer": !isCorrectAnswer});
	};

	handlePinnedToTopMark = (commentId, isPinnedToTop) => {
		this.updateComment(commentId, () => ({
			isPinnedToTop: !isPinnedToTop,
		}));

		this.debouncedSendData(this.props.commentsApi.updateComment, commentId, {isPinnedToTop: !isPinnedToTop});
	};

	handleShowEditForm = (commentId) => {
		this.setState({
			commentEditing: {
				...this.state.commentEditing,
				commentId: commentId,
			},
		});
	};

	handleShowReplyForm = (commentId) => {
		this.setState({
			reply: {
				...this.state.reply,
				commentId: commentId,
			},
		});
	};

	handleEditComment = async (commentId, text) => {
		const {commentsApi} = this.props;

		this.setState({
			commentEditing: {
				commentId: commentId,
				sending: true,
			},
		});

		try {
			const newComment = await commentsApi.updateComment(commentId, {"text": text});

			this.updateComment(commentId, () => ({
				text: text,
				renderedText: newComment.renderedText,
			}));

			this.setState({
				commentEditing: {
					commentId: null,
				},
			});
		} catch (e) {
			Toast.push("Не удалось изменить комментарий. Попробуйте снова.");
			this.setState({
				commentEditing: {
					commentId: commentId,
				},
			});
			console.error(e);
		} finally {
			this.setState({
				commentEditing: {
					sending: false,
				},
			})
		}
	};

	handleAddReplyComment = async (commentId, text) => {
		const {commentsApi, courseId, slideId, forInstructors} = this.props;

		this.setState({
			animation: true,
			reply: {
				commentId: commentId,
				sending: true,
			}
		});

		try {
			const newReply = await commentsApi.addComment(courseId, slideId, text, commentId, forInstructors);
			const index = this.state.threads.findIndex(comment => comment.id === commentId);
			const comment = this.state.threads[index];
			const newReplies = comment.replies.concat(newReply);
			const newComment = {...comment, replies: newReplies};

			this.setState({
				threads: [
					...this.state.threads.slice(0, index),
					newComment,
					...this.state.threads.slice(index + 1),
				],
				reply: {
					commentId: null,
				},
			});
		} catch (e) {
			Toast.push("Не удалось отправить комментарий. Попробуйте снова.");
			this.setState({
				reply: {
					commentId: commentId,
				},
			});
			console.error(e);
		} finally {
			this.setState({
				reply: {
					sending: false,
				}
			});
		}
	};

	handleDeleteComment = (comment) => {
		const threads = JSON.parse(JSON.stringify(this.state.threads));
		const {forInstructors, handleInstructorsCommentCount} = this.props;

		this.setState({
			animation: true,
		});

		const restoreComment = this.deleteComment(comment.id, threads);
		this.setState({threads});

		try {
			this.props.commentsApi.deleteComment(comment.id);

			if (forInstructors && !comment.parentCommentId) {
				handleInstructorsCommentCount("delete");
			}

			Toast.push("Комментарий удалён", {
				label: "Восстановить",
				handler: () => {
					restoreComment();
					this.setState({ threads });

					if (forInstructors && !comment.parentCommentId) {
						handleInstructorsCommentCount("add");
					}

					Toast.push("Комментарий восстановлен");

					this.props.commentsApi.updateComment(comment.id);
				}
			})
		} catch (e) {
			Toast.push("Комментарий не удалён. Произошла ошибка, попробуйте снова");
			this.setState({threads});
			console.error(e);
		}
	};

	handleScroll = (ref) => {
		scrollToView(ref);
	};

	getUserSolutionsUrl = (userId) => {
		const {courseId, slideId} = this.props;
		return `${window.location.origin}/Analytics/UserSolutions?courseId=${courseId}&slideId=${slideId}&userId=${userId}`;
	};

	sendData = (method, commentId, property) =>
		method(commentId, property)
		.catch(e => {
			Toast.push("Не удалось изменить комментарий. Произошла ошибка, попробуйте снова");
			console.error(e);
		});
}

CommentsList.propTypes = {
	user: userType.isRequired,
	userRoles: userRoles.isRequired,
	slideId: PropTypes.string.isRequired,
	slideType: PropTypes.string.isRequired,
	forInstructors: PropTypes.bool,
	handleInstructorsCommentCount: PropTypes.func,
	commentsApi: PropTypes.objectOf(PropTypes.func),
	commentPolicy: commentPolicy,
	courseId: PropTypes.string.isRequired,
};

export default CommentsList;
