import React, { Component } from 'react';
import PropTypes from "prop-types";
import { userType, userRoles } from "../commonPropTypes";
import { TransitionGroup, CSSTransition } from 'react-transition-group';
//import * as debounce from "debounce";
import Icon from "@skbkontur/react-icons";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import Thread from "../Thread/Thread";
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import Error404 from "../../common/Error/Error404";
import Stub from "../Stub/Stub";


import styles from "./CommentsList.less";

class CommentsList extends Component {
	constructor(props) {
		super(props);

		this.state = {
			newCommentId: 1,
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
			loadedComments: false,
			status: '',
		};

		this.myRef = React.createRef();

		//this.debouncedSendData = debounce(this.sendData, 300);
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
			let comments = json.topLevelComments;
			this.setState({
				threads: comments,
				loadingComments: false,
			});
		})
		.catch(() => {
			this.setState({
				status: 'error',
			});
		})
		.finally(() => {
			this.setState({
				loadingComments: false,
			})
		});
	};

	render() {
		const {threads, commentEditing, reply, sending, status, newCommentId, loadingComments} = this.state;
		const {user, userRoles, courseId, slideId} = this.props;

		if (this.state.status === "error") {
			return <Error404 />;
		}

		return (
			<>
				{ threads.length === 0 && this.renderStubForEmptyComments()}
				<>
					{!user.isAuthenticated ? <Stub courseId={courseId} slideId={slideId} /> :
					<CommentSendForm
						ref={this.myRef}
						commentId={newCommentId}
						author={user}
						handleSubmit={this.handleAddComment}
						sending={sending}
						isSuccessSend={status} />}
					<Loader type="big" active={loadingComments} />
					<TransitionGroup>
						{threads.map((comment) =>
						<CSSTransition
							key={comment.id}
							mountOnEnter
							unmountOnExit
							classNames={{
								enter: styles.enter,
								exit: styles.exit,
								enterActive: styles.enterActive,
								exitActive: styles.exitActive,
							}}
							timeout={1000}>
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
								}}
								getUserSolutionsUrl={this.getUserSolutionsUrl}
								user={user}
								userRoles={userRoles} />
						</CSSTransition>)}
					</TransitionGroup>
					{user.isAuthenticated &&
					<button className={styles.sendButton} onClick={this.handleScrollToTop}>
						<Icon name="CommentLite" color="#3072C4" />
						<span className={styles.sendButtonText}>Оставить комментарий</span>
					</button>}
				</>
			</>
		)
	}

	renderStubForEmptyComments() {
		return (
			<>
				{!this.props.forInstructors ?
				<p className={styles.emptyComments}>
					К этому слайду ещё нет коммаентариев. Вы можете начать беседу со студентами,
					добавив комментарий.
				</p>
				:
				<p className={styles.emptyComments}>
					К этому слайду нет комментариев преподавателей. Вы можете начать беседу с преподавателями,
					добавив комментарий.
				</p>}
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
			const threads = JSON.parse(JSON.stringify(this.state.threads));
			const thread = threads.find(thread => thread.id === id);

			Object.assign(thread, reducer(thread));

			return threads;
		});
	}

	updateComment(id, reducer) {
		this.setState(() => {
			const threads = JSON.parse(JSON.stringify(this.state.threads));
			const comment = this.findComment(id, threads);

			Object.assign(comment, reducer(comment));

			return threads;
		});
	}

	handleAddComment = async (commentId, text) => {
		const {commentsApi, courseId, slideId, forInstructors} = this.props;
		const threads = JSON.parse(JSON.stringify(this.state.threads));

		this.setState({
			sending: true,
		});

		try {
			const request = await commentsApi.addComment(courseId, slideId, text, null, forInstructors);
			const newComment = await commentsApi.getComment(request.id);

			this.setState({
				threads: [newComment, ...threads],
				sending: false,
				status: 'success',
				newCommentId: this.state.newCommentId + 1,
			});
		}
		catch (e) {
			console.log(e);
		}
		finally {
			this.setState({
				sending: false,
			})
		}
	};

	handleLikeClick = (commentId, isLiked) => {
		const {commentsApi} = this.props;

		this.updateComment(commentId, ({isLiked, likesCount}) => ({
			likesCount: isLiked ? likesCount - 1 : likesCount + 1,
			isLiked: !isLiked,
		}));

		if (!isLiked) {
			commentsApi.likeComment(commentId)
				.catch(console.error);
			// this.debouncedSendData(commentsApi.likeComment, commentId);
		} else {
			commentsApi.dislikeComment(commentId)
				.catch(console.error);
		}
		// this.debouncedSendData(commentsApi.dislikeComment, commentId);
	};

	handleApprovedMark = (commentId, isApproved) => {

		this.updateComment(commentId, ({isApproved}) => ({
			isApproved: !isApproved,
		}));

		// this.debouncedSendData(commentsApi.updateComment, commentId, isApproved);
		this.props.commentsApi.updateComment(commentId, {'isApproved': !isApproved})
			.catch(console.error);
	};

	handleCorrectAnswerMark = (commentId, isCorrectAnswer) => {
		this.updateComment(commentId, ({isCorrectAnswer}) => ({
			isCorrectAnswer: !isCorrectAnswer,
		}));

		// this.debouncedSendData(commentsApi.updateComment, commentId, isCorrectAnswer);
		this.props.commentsApi.updateComment(commentId, {'isCorrectAnswer': !isCorrectAnswer})
		.catch(console.error);
	};

	handlePinnedToTopMark = (commentId, isPinnedToTop) => {
		this.updateComment(commentId, ({isPinnedToTop}) => ({
			isPinnedToTop: !isPinnedToTop,
		}));

		const comment = this.state.threads.find(comment => comment.id === commentId);
		const filteredComments = this.state.threads.filter(comment => comment.id !== commentId);

		this.setState({
			threads: [comment, ...filteredComments],
		});

		// this.debouncedSendData(commentsApi.updateComment, commentId, isPinnedToTop);
		this.props.commentsApi.updateComment(commentId, {'isPinnedToTop': !isPinnedToTop})
			.catch(console.error);
	};

	handleShowEditForm = (commentId) => {
		this.setState({
			commentEditing: {
				...this.state.commentEditing,
				commentId: commentId,
			}
		});
	};

	handleShowReplyForm = (commentId) => {
		this.setState({
			reply: {
				...this.state.reply,
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
				commentId: commentId,
				sending: true,
			}
		});

		// this.debouncedSendData(commentsApi.updateComment, commentId, text);
		this.props.commentsApi.updateComment(commentId, {'text': text})
			.then(() =>
				this.setState({
					commentEditing: {
						commentId: null,
						sending: false,
					},
				})
				// TODO: добавить toast c сообщением об ошибке, если запрос не отправился (commentId: не убирать)
			)
			.catch(console.error)
			.finally(() =>
				this.setState({
					commentEditing: {
						sending: false,
					},
				})
			)
	};

	handleAddReplyComment = async (commentId, text) => {
		const {commentsApi, courseId, slideId, forInstructors} = this.props;

		this.updateComment(commentId, () => ({
			text,
		}));

		this.setState({
			reply: {
				commentId: commentId,
				sending: true,
			}
		});

		const request = await commentsApi.addComment(courseId, slideId, text, commentId, forInstructors);
		const newReply = await commentsApi.getComment(request.id);

		const comment = this.state.threads.find(comment => comment.id === commentId);
		const newReplies = comment.replies.concat(newReply);
		const newComment = {...comment, replies: newReplies};
		const filteredThreads = this.state.threads.filter(comment => comment.id !== commentId);

		try {
			this.setState({
				threads: [newComment, ...filteredThreads],
				reply: {
					commentId: null,
					sending: false,
				}
			});
		} catch (e) {
			console.log(e)
		} finally {
			this.setState({
				reply: {
					sending: false,
				}
			});
		}

		if (!request) {
			this.setState({
				reply: {
					commentId: commentId,
					sending: false,
				}
			});
		}
	};

	handleDeleteComment = (commentId) => {
		const threads = JSON.parse(JSON.stringify(this.state.threads));
		const queue = [threads];

		while (queue.length > 0) {
			const thread = queue.shift();
			const comment = thread.find(c => c.id === commentId);
			const index = thread.findIndex(c => c.id === commentId);

			if (index !== -1) {
				thread.splice(index, 1);
				this.setState({threads});

				this.props.commentsApi.deleteComment(commentId)
				.catch(console.error);

				Toast.push("Комментарий удалён", {
					label: "Восстановить",
					handler: () => {
						thread.splice(index, 0, comment);
						this.setState({threads});

						this.props.commentsApi.updateComment(commentId)
						.catch(console.error);
						Toast.push("Комментарий восстановлен");
					}
				});
				return;
			}

			thread.forEach(comment => {
				queue.push(comment.replies);
			});
		}

		throw new Error(`Comment with id ${commentId} not found`);
	};

	handleScrollToTop = () => {
		window.scrollTo(0, this.myRef.current.offsetTop);
	};

	getUserSolutionsUrl = (userId) => {
		const {courseId, slideId} = this.props;
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
	commentsApi: PropTypes.objectOf(PropTypes.func),
	courseId: PropTypes.string.isRequired,
	slideId: PropTypes.string.isRequired,
};

export default CommentsList;