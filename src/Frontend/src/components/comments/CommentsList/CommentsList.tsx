import React, { Component } from "react";
import { debounce } from "debounce";
import { CSSTransition, TransitionGroup } from "react-transition-group";

import api from "src/api";

import { CommentLite } from "icons";
import { Toast } from "ui";
import CourseLoader from "src/components/course/Course/CourseLoader/CourseLoader";
import Thread from "../Thread/Thread";
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import Stub from "../Stub/Stub";
import Error404 from "../../common/Error/Error404";

import scrollToView from "src/utils/scrollToView";
import { buildQuery } from "src/utils";

import { TabsType } from "src/consts/tabsType";
import { AccountState } from "src/redux/account";
import { UserRolesWithCourseAccesses } from "src/utils/courseRoles";
import { Comment, CommentPolicy } from "src/models/comments";
import { SlideType } from "src/models/slide";
import { CommentStatus } from "src/consts/comments";
import { userSolutions } from "src/consts/routes";

import styles from "./CommentsList.less";


const defaultCommentsData: {
	commentsPerPack: number;
	scrollDistance: number;
	threadsToRender: Comment[],
	repliesToRender: Comment[],
} = {
	commentsPerPack: 15,
	scrollDistance: 500,
	threadsToRender: [],
	repliesToRender: [],
};

function throttle(fn: () => void, wait: number) {
	let time = Date.now();
	return function () {
		if((time + wait - Date.now()) < 0) {
			fn();
			time = Date.now();
		}
	};
}

interface Props {
	key?: string;
	commentsApi: typeof api.comments;
	headerRef: React.RefObject<HTMLDivElement>;

	courseId: string;
	slideId: string;
	slideType: SlideType;

	user: AccountState;
	userRoles: UserRolesWithCourseAccesses;

	forInstructors: boolean;
	isSlideReady: boolean;

	commentPolicy: CommentPolicy | null;
	handleInstructorsCommentCount: (operation: 'add' | string) => void;
	handleTabChange: (tab: TabsType, bool: boolean) => void;
}

interface State {
	status: string;
	newCommentId: number;
	saveCommentLikeStatus: null,
	threads: Comment[],
	commentEditing: CommentStatus;
	reply: CommentStatus;

	showFocus: boolean;
	sending: boolean;
	animation: boolean;
	loadingComments: boolean;
}

export interface ActionsType {
	handleLikeClick: (commentId: string, isLiked: boolean) => void;
	handleCorrectAnswerMark: (commentId: string, isRightAnswer: boolean) => void;
	handleApprovedMark: (commentId: string, isApproved: boolean) => void;
	handlePinnedToTopMark: (commentId: string, isPinnedToTop: boolean) => void;
	handleEditComment: (commentId: string, text: string) => void;
	handleAddReplyComment: (commentId: string, text: string) => void;
	handleDeleteComment: (comment: Comment) => void;
	handleShowEditForm: (commentId: string | null) => void;
	handleShowReplyForm: (commentId: string | null) => void;
}

class CommentsList extends Component<Props, State> {
	private lastThreadRef: React.RefObject<HTMLDivElement> = React.createRef();
	private commentsListRef: React.RefObject<HTMLDivElement> = React.createRef();
	private commentsData: typeof defaultCommentsData = { ...defaultCommentsData };
	private throttleScroll: (() => void) | null = null;
	private readonly debouncedSendData: null
		| ((method: (commentId: string) => Promise<Comment> | Promise<string>, commentId: string,
		property?: Record<string, unknown>
	) => void) = null;

	constructor(props: Props) {
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
			showFocus: false,
			sending: false,
			loadingComments: false,
			status: "",
			animation: false,
		};

		this.debouncedSendData = debounce(this.sendData, 300);
	}

	componentDidMount(): void {
		const { courseId, slideId, forInstructors } = this.props;
		this.loadComments(courseId, slideId, forInstructors)
			.then(() => {
				if(window.location.hash.includes("#comment")) {
					this.handleScrollToCommentByHashFormUrl();
				}
			});

		this.throttleScroll = throttle(this.handleScrollToBottom, 100);

		window.addEventListener("scroll", this.throttleScroll);
		window.addEventListener("hashchange", this.handleScrollToCommentByHashFormUrl);
	}

	componentWillUnmount(): void {
		if(this.throttleScroll) {
			window.removeEventListener("scroll", this.throttleScroll);
		}
		window.removeEventListener("hashchange", this.handleScrollToCommentByHashFormUrl);
	}

	componentDidUpdate(prevProps: Props): void {
		const { slideId, courseId, forInstructors, } = this.props;

		if(slideId !== prevProps.slideId) {
			this.commentsData = { ...defaultCommentsData };
			this.setStateIfMounted({
				threads: [],
			});
			this.loadComments(courseId, slideId, forInstructors);
		}
	}

	get commentIds(): string[] {
		const { threads } = this.state;
		return this.getAllCommentsIds(threads);
	}

	getAllCommentsIds(threads: Comment[]): string[] {
		const commentIds = [];

		for (let i = 0; i < threads.length; i++) {
			const thread = threads[i];
			commentIds.push(thread.id);

			if(!thread.replies) {
				continue;
			}

			for (let j = 0; j < thread.replies.length; j++) {
				const reply = thread.replies[j];
				commentIds.push(reply.id);
			}
		}

		return commentIds;
	}

	loadComments = (courseId: string, slideId: string,
		forInstructors: boolean, offset?: number, count?: number
	): Promise<Comment[]> => {
		const { loadingComments, } = this.state;
		const { commentsApi, } = this.props;

		if(loadingComments) {
			return Promise.reject();
		}

		this.setStateIfMounted({
			loadingComments: true,
		});

		const commentsApiRequest = commentsApi.getComments(courseId, slideId, forInstructors, offset, count)
			.then(json => {
				const comments = json.topLevelComments;

				this.setStateIfMounted({
					loadingComments: false,
				});
				this.commentsData.threadsToRender = comments;

				this.renderPackOfComments();
				return comments;
			});

		commentsApiRequest
			.catch(() => {
				this.setStateIfMounted({
					status: "error",
				});
			})
			.finally(() => {
				this.setStateIfMounted({
					loadingComments: false,
				});
			});

		return commentsApiRequest;
	};

	renderPackOfComments(): void {
		const { threadsToRender, repliesToRender, commentsPerPack } = this.commentsData;
		const { threads } = this.state;
		const newThreads = [...threads];
		let countOfCommentsToRender = 0;
		if(repliesToRender.length > 0) {
			const lastThread = newThreads[newThreads.length - 1];
			const newReplies = repliesToRender.splice(0, commentsPerPack);
			countOfCommentsToRender += newReplies.length;
			lastThread.replies = [...lastThread.replies, ...newReplies];
			lastThread.replies.sort(
				(r1, r2) => new Date(r1.publishTime).getTime() - new Date(r2.publishTime).getTime());
		}

		while (countOfCommentsToRender < commentsPerPack && threadsToRender.length !== 0) {
			const thread = threadsToRender.shift();
			if(!thread) {
				break;
			}
			countOfCommentsToRender++;
			const threadReplies = thread.replies;
			const countOfCommentsLeftInPack = commentsPerPack - countOfCommentsToRender;

			if(threadReplies.length > countOfCommentsLeftInPack) {
				thread.replies = threadReplies.splice(0, countOfCommentsLeftInPack);
				countOfCommentsToRender += countOfCommentsLeftInPack;
				newThreads.push(thread);
				this.commentsData.repliesToRender = threadReplies;
			} else {
				countOfCommentsToRender += thread.replies.length;
				newThreads.push(thread);
			}
		}

		this.setStateIfMounted({
			threads: newThreads,
		});
	}

	setStateIfMounted(updater: Partial<State>, callback?: () => void): void {
		if(this.commentsListRef.current) {
			this.setState(updater as State, callback);
		}
	}

	handleScrollToBottom = (): void => {
		const { scrollDistance, } = this.commentsData;

		const element = document.documentElement;
		const windowRelativeBottom = element.getBoundingClientRect().bottom;
		if(windowRelativeBottom < element.clientHeight + scrollDistance) {
			this.renderPackOfComments();
		}
	};

	handleScrollToCommentByHashFormUrl = (): void => {
		const { courseId, slideId, forInstructors, handleTabChange, } = this.props;

		if(window.location.hash.includes("#comment")) {
			const startIndex = window.location.hash.indexOf('-') + 1;
			const commentIdFromHash = window.location.hash.slice(startIndex);
			const nameChangesTab = forInstructors ? TabsType.allComments : TabsType.instructorsComments;

			const { threadsToRender, repliesToRender } = this.commentsData;
			const notRenderedComments = [...repliesToRender, ...threadsToRender];
			const notRenderedCommentIds = this.getAllCommentsIds(notRenderedComments);
			const allCommentIds = [...this.commentIds, ...notRenderedCommentIds];
			const indexOfComment = notRenderedCommentIds.indexOf(commentIdFromHash);
			if(indexOfComment > 0) {
				const commentsPerPack = this.commentsData.commentsPerPack;
				this.commentsData.commentsPerPack = indexOfComment + commentsPerPack;
				this.renderPackOfComments();
				this.commentsData.commentsPerPack = commentsPerPack;
			} else if(!allCommentIds.includes(commentIdFromHash)) {
				handleTabChange(nameChangesTab, false);
				this.loadComments(courseId, slideId, forInstructors);
				this.setStateIfMounted({
					threads: [],
				});
			}
		}
	};

	render(): React.ReactNode {
		const { threads, loadingComments } = this.state;
		const { user, courseId, slideId, commentPolicy, key, } = this.props;
		const replies = threads.reduce((sum, current) => sum + current.replies.length, 0);

		if(this.state.status === "error") {
			return <Error404/>;
		}

		if(!courseId || !slideId) {
			return null;
		}

		return (
			<div key={ key } ref={ this.commentsListRef }>
				{ loadingComments ?
					<CourseLoader isSlideLoader={ false }/> :
					<>
						{ !user.id &&
						<Stub hasThreads={ threads.length > 0 } courseId={ courseId } slideId={ slideId }/> }
						{ (user.id && commentPolicy && !commentPolicy.areCommentsEnabled) && this.renderMessageIfCommentsDisabled() }
						{ this.renderSendForm() }
						{ this.renderThreads() }
					</> }
				{ (commentPolicy && commentPolicy.areCommentsEnabled && user.id && (threads.length + replies) > 7) &&
				<button className={ styles.sendButton } onClick={ this.handleShowSendForm }>
					<CommentLite color="#3072C4"/>
					Оставить комментарий
				</button> }
			</div>
		);
	}

	renderMessageIfCommentsDisabled(): React.ReactElement {
		return (
			<div className={ styles.textForDisabledComments }>
				В данный момент комментарии выключены.
			</div>
		);
	}

	renderSendForm(): React.ReactNode {
		const { sending, status, newCommentId, showFocus } = this.state;
		const { user, forInstructors, commentPolicy } = this.props;
		const focusedSendForm = { inSendForm: showFocus, };

		return (
			user.id && commentPolicy && (commentPolicy.areCommentsEnabled || commentPolicy.onlyInstructorsCanReply) &&
			<CommentSendForm
				key={ showFocus }
				isShowFocus={ focusedSendForm }
				isForInstructors={ forInstructors }
				commentId={ newCommentId }
				author={ user }
				handleSubmit={ this.handleAddComment }
				sending={ sending }
				sendStatus={ status }/>
		);
	}

	renderThreads(): React.ReactElement {
		const { threads, commentEditing, reply, animation } = this.state;
		const { user, userRoles, slideType, courseId, commentPolicy, isSlideReady, } = this.props;

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

		const actions: ActionsType = {
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
			<TransitionGroup enter={ animation }>
				{ threads
					.map((comment) =>
						<CSSTransition
							key={ comment.id }
							appear
							in={ animation }
							mountOnEnter
							unmountOnExit
							classNames={ transitionStyles }
							timeout={ duration }>
							<section className={ styles.thread } key={ comment.id } ref={ this.lastThreadRef }>
								<Thread
									user={ user }
									userRoles={ userRoles }
									slideType={ slideType }
									courseId={ courseId }
									animation={ animation }
									comment={ comment }
									commentPolicy={ commentPolicy }
									commentEditing={ commentEditing }
									reply={ reply }
									actions={ actions }
									getUserSolutionsUrl={ this.getUserSolutionsUrl }
									isSlideReady={ isSlideReady }/>
							</section>
						</CSSTransition>) }
			</TransitionGroup>
		);
	}

	findCommentPosition(id: string, threads: Comment[]): { parentArray: Comment[], index: number } | undefined {
		for (let i = 0; i < threads.length; i++) {
			const thread = threads[i];
			if(thread.id === id) {
				return { parentArray: threads, index: i };
			}

			if(!thread.replies) {
				continue;
			}

			for (let j = 0; j < thread.replies.length; j++) {
				const reply = thread.replies[j];
				if(reply.id === id) {
					return { parentArray: thread.replies, index: j };
				}
			}
		}
	}

	findComment(id: string, threads: Comment[]): Comment | undefined {
		const commentPosition = this.findCommentPosition(id, threads);

		if(commentPosition) {
			return commentPosition.parentArray[commentPosition.index];
		}
	}

	deleteComment(id: string, threads: Comment[]): (() => void) | undefined {
		const commentPosition = this.findCommentPosition(id, threads);
		if(commentPosition) {
			const { parentArray, index } = commentPosition;
			const comment = parentArray[index];
			parentArray.splice(index, 1);

			return () => {
				parentArray.splice(index, 0, comment);
			};
		}
	}

	compareThreads(a: Comment, b: Comment): number {
		if(a.isPinnedToTop && !b.isPinnedToTop) {
			return -1;
		}

		if(!a.isPinnedToTop && b.isPinnedToTop) {
			return 1;
		}

		if(a.publishTime > b.publishTime) {
			return -1;
		}

		if(a.publishTime < b.publishTime) {
			return 1;
		}

		return 0;
	}

	updateComment(id: string, reducer: (comment: Comment) => Partial<Comment>): void {
		const threads = [...this.state.threads];
		const comment = this.findComment(id, threads);

		if(comment) {
			Object.assign(comment, reducer(comment));

			if(!comment.parentCommentId) {
				threads.sort(this.compareThreads);
			}
		}

		this.setState({ threads });
	}

	handleAddComment = async (commentId: string, text: string): Promise<void> => {
		const { commentsApi, courseId, slideId, forInstructors, handleInstructorsCommentCount } = this.props;
		const threads = [...this.state.threads];

		this.setState({
			sending: true,
			animation: true,
		});

		try {
			const newComment = await commentsApi.addComment(courseId, slideId, text, null, forInstructors);
			const pinnedComments = threads.filter(comment => comment.isPinnedToTop);
			const filteredComments = threads.filter(comment => !comment.isPinnedToTop)
				.sort((a, b) => new Date(b.publishTime).getTime() - new Date(a.publishTime).getTime());

			this.setState({
				threads: [
					...pinnedComments,
					newComment,
					...filteredComments,
				],
				newCommentId: this.state.newCommentId + 1,
			});

			this.handleScroll(this.lastThreadRef);

			if(forInstructors) {
				handleInstructorsCommentCount("add");
			}
		} catch (e) {
			Toast.push("Не удалось добавить комментарий. Попробуйте снова.");
			this.setState({
				newCommentId: this.state.newCommentId,
			});
			console.error(e);
		} finally {
			this.setState({
				sending: false,
			});
		}
	};

	handleLikeClick = (commentId: string, isLiked: boolean): void => {
		const { commentsApi } = this.props;

		this.updateComment(commentId, ({ likesCount }) => ({
			likesCount: isLiked ? likesCount - 1 : likesCount + 1,
			isLiked: !isLiked,
		}));

		this.debouncedSendData?.(isLiked ? commentsApi.dislikeComment : commentsApi.likeComment, commentId);
	};

	handleApprovedMark = (commentId: string, isApproved: boolean): void => {
		this.updateComment(commentId, () => ({
			isApproved: !isApproved,
		}));

		this.debouncedSendData?.(this.props.commentsApi.updateComment, commentId, { "isApproved": !isApproved });
	};

	handleCorrectAnswerMark = (commentId: string, isCorrectAnswer: boolean): void => {
		this.updateComment(commentId, () => ({
			isCorrectAnswer: !isCorrectAnswer,
		}));

		this.debouncedSendData?.(this.props.commentsApi.updateComment, commentId,
			{ "isCorrectAnswer": !isCorrectAnswer });
	};

	handlePinnedToTopMark = (commentId: string, isPinnedToTop: boolean): void => {
		this.updateComment(commentId, () => ({
			isPinnedToTop: !isPinnedToTop,
		}));

		this.debouncedSendData?.(this.props.commentsApi.updateComment, commentId, { isPinnedToTop: !isPinnedToTop });
	};

	handleShowSendForm = (): void => {
		this.handleScroll(this.props.headerRef);
		this.setState({
			showFocus: true,
		});
	};

	handleShowEditForm = (commentId: string | null): void => {
		this.setState({
			commentEditing: {
				...this.state.commentEditing,
				commentId: commentId,
			},
		});
	};

	handleShowReplyForm = (commentId: string | null): void => {
		this.setState({
			reply: {
				...this.state.reply,
				commentId: commentId,
			},
		});
	};

	handleEditComment = async (commentId: string, text: string): Promise<void> => {
		const { commentsApi } = this.props;

		this.setState({
			commentEditing: {
				commentId: commentId,
				sending: true,
			},
		});

		try {
			const newComment = await commentsApi.updateComment(commentId, { "text": text });

			this.updateComment(commentId, () => ({
				text: text,
				renderedText: newComment.renderedText,
			}));

			this.setState({
				commentEditing: {
					commentId: null,
					sending: false,
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
			});
		}
	};

	handleAddReplyComment = async (commentId: string, text: string): Promise<void> => {
		const { commentsApi, courseId, slideId, forInstructors } = this.props;

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
			const newComment = { ...comment, replies: newReplies };

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

	handleDeleteComment = (comment: Comment): void => {
		const threads = JSON.parse(JSON.stringify(this.state.threads));
		const { forInstructors, handleInstructorsCommentCount, commentsApi, } = this.props;

		this.setState({
			animation: true,
		});

		const restoreComment = this.deleteComment(comment.id, threads);
		this.setState({ threads });

		try {
			commentsApi.deleteComment(comment.id);

			if(forInstructors && !comment.parentCommentId) {
				handleInstructorsCommentCount("delete");
			}

			Toast.push("Комментарий удалён", {
				label: "Восстановить",
				handler: () => {
					restoreComment?.();
					this.setState({ threads });

					if(forInstructors && !comment.parentCommentId) {
						handleInstructorsCommentCount("add");
					}

					Toast.push("Комментарий восстановлен");

					commentsApi.updateComment(comment.id);
				}
			});
		} catch (e) {
			Toast.push("Комментарий не удалён. Произошла ошибка, попробуйте снова");
			this.setState({ threads });
			console.error(e);
		}
	};

	handleScroll = (ref: React.RefObject<HTMLDivElement>): void => {
		scrollToView(ref);
	};

	getUserSolutionsUrl = (userId: string): string => {
		const { courseId, slideId, } = this.props;
		return userSolutions + buildQuery({ courseId, slideId, userId });
	};

	sendData = (method: (commentId: string, property: unknown) => Promise<unknown>, commentId: string,
		property: unknown
	): Promise<unknown> =>
		method(commentId, property)
			.catch(e => {
				Toast.push("Не удалось изменить комментарий. Произошла ошибка, попробуйте снова");
				console.error(e);
			});
}

export default CommentsList;
