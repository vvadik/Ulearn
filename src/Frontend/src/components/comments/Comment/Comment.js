import React, { Component, createContext } from 'react';
import PropTypes from "prop-types";
import moment from "moment";
import * as debounce from "debounce";
import Hint from "@skbkontur/react-ui/components/Hint/Hint";
import Avatar from "../../common/Avatar/Avatar";
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import Like from "./Like/Like";
import InstructorActions from "./Kebab/InstructorActions";
import Header from "./Header/Header";
import Marks from "./Marks/Marks";
import Actions from "./Actions/Actions";
import { CommentContext } from "../CommentsList/CommentsList";
import { CommentActionHandlers } from './commonPropTypes';

import styles from "./Comment.less";


class Comment extends Component {
	constructor(props) {
		super(props);
		this.state = {
			showEditForm: false,
			likesCount: props.comment.likesCount,
			isLiked: props.comment.isLiked,
			isCorrectAnswer: props.comment.isCorrectAnswer,
			isApproved: props.comment.isApproved,
			isPinnedToTop: props.comment.isPinnedToTop,
		};

		this.debouncedSendData = debounce(this.sendData, 300);
	};

	render() {
		// const { dispatch } = useContext(CommentContext);
		const {children, comment, userRoles} = this.props;
		const {likesCount, isLiked, isApproved, isCorrectAnswer, isPinnedToTop, showEditForm} = this.state;

		return (
			<div className={styles.comment}>
				<Avatar user={comment.author} size='big' />
				<div className={styles.content}>
					<Header name={comment.author.visibleName}>
						<Like checked={isLiked} count={likesCount} onClick={this.handleLikeClick} />
						<Marks
							isApproved={isApproved}
							isCorrectAnswer={isCorrectAnswer}
							isPinnedToTop={isPinnedToTop} />
						{this.canModerateComments(userRoles, 'editPinAndRemoveComments') ?
							<InstructorActions
								handleShowEditComment={this.handleShowEditComment}
								commentActions={this.commentActionsHandlers} isApproved={isApproved} /> : null}
					</Header>
					<Hint pos="bottom" text={comment.publishTime} disableAnimations={false} useWrapper={false}>
						<div className={styles.timeSinceAdded}>
							{moment(comment.publishTime).fromNow()}
						</div>
					</Hint>
					{showEditForm ? this.renderEditCommentForm() : this.renderComment()}
					{children}
				</div>
			</div>
		);
	}

	renderComment() {
		const {isCorrectAnswer, isPinnedToTop} = this.state;
		const {comment, commentActions, user, userRoles, url, hasReplyAction} = this.props;
		console.log('inComment', commentActions);
		return (
			<React.Fragment>
				<p className={styles.text}>
					<span dangerouslySetInnerHTML={{__html: comment.renderedText}} />
				</p>
				<Actions
					author={comment.author}
					user={user}
					userRoles={userRoles}
					url={url}
					hasReplyAction={hasReplyAction}
					parentCommentId={comment.parentCommentId}
					isCorrectAnswer={isCorrectAnswer}
					isPinnedToTop={isPinnedToTop}
					commentActions={this.commentActionsHandlers}
					canModerateComments={this.canModerateComments} />
			</React.Fragment>
		)
	}

	renderEditCommentForm() {
		const {comment, sending} = this.props;

		return (
			<CommentSendForm
				text={comment.commentText}
				autofocus
				onSubmit={this.handleEditComment}
				submitTitle={'Сохранить'}
				onCancel={this.handleEditCancel}
				sending={sending} />
		)
	}

	canModerateComments = (role, accesses) => {
		return role.isSystemAdministrator || role.courseRole === 'CourseAdmin' ||
			(role.courseRole === 'Instructor' && role.courseAccesses.includes(accesses))
	};

	handleLikeClick = () => {
		const {isLiked, likesCount} = this.state;
		const {comment, commentActions} = this.props;

		if (!isLiked) {
			this.setState({
				likesCount: likesCount + 1,
				isLiked: true,
			})
		} else {
			this.setState({
				likesCount: likesCount - 1,
				isLiked: false,
			})
		}

		this.debouncedSendData(commentActions.handleLikeChanged, comment.id, isLiked);
	};

	handleShowReplyForm = () => {
		this.props.commentActions.handleShowReplyForm();
	};

	handleShowEditComment = () => {
		this.setState({
			showEditForm: true,
		});
	};

	handleEditCancel = () => {
		this.setState({showEditForm: false});
	};

	handleEditComment = (text) => {
		const {comment, commentActions} = this.props;

		this.setState({
			showEditForm: false,
		});

		commentActions.handleEditComment(comment.id, text);
	};

	handleCorrectAnswerMark = () => {
		const {isCorrectAnswer} = this.state;
		const {comment, commentActions} = this.props;
		console.log(commentActions);

		this.setState({
			isCorrectAnswer: !isCorrectAnswer,
		});

		this.debouncedSendData(commentActions.handleCorrectAnswerMark, comment.id, isCorrectAnswer);
	};

	handlePinnedToTopMark = () => {
		const {isPinnedToTop} = this.state;
		const {comment, commentActions} = this.props;

		this.setState({
			isPinnedToTop: !isPinnedToTop,
		});

		this.debouncedSendData(commentActions.handlePinnedToTopMark, comment.id, isPinnedToTop);
	};

	handleVisibleMark = () => {
		const {isApproved} = this.state;
		const {comment, commentActions} = this.props;

		this.setState({
			isApproved: !isApproved,
		});

		this.debouncedSendData(commentActions.handleVisibleMark, comment.id, isApproved);
	};

	handleDeleteComment = () => {
		const {comment, commentActions} = this.props;
		this.props.commentActions.handleDeleteComment();
	};

	sendData = (action, value, flag) => {
		return () => action(value, flag);
	};

	commentActionsHandlers = {
		handleShowEditComment: this.handleShowEditComment,
		handleShowReplyForm: this.handleShowReplyForm,
		handleCorrectAnswerMark: this.handleCorrectAnswerMark,
		handlePinnedToTopMark: this.handlePinnedToTopMark,
		handleVisibleMark: this.handleVisibleMark,
		handleEditComment: this.handleEditComment,
		handleDeleteComment: this.handleDeleteComment,
	};
}

const userRolesModel = PropTypes.shape({
	isSystemAdministrator: PropTypes.bool,
	courseRole: PropTypes.string.isRequired,
	courseAccesses: PropTypes.arrayOf(PropTypes.string),
});

Comment.propTypes = {
	/** Идентифицирует комментарий, с которым работает компонент.
	 * При изменении идентификатора текст в поле ввода очищается. При сохранении того же идентификатора - текст сохраняется. */
	user: Avatar.propTypes.user,
	userRoles: userRolesModel,
	comment: PropTypes.object,
	children: PropTypes.array,
	url: PropTypes.string,
	sending: PropTypes.bool,
	hasReplyAction: PropTypes.bool,
	commentActions: PropTypes.object,
	showReplyForm: PropTypes.func,
	onLikeChanged: PropTypes.func,
	hideComment: PropTypes.func,
	markAsCorrectAnswer: PropTypes.func,
	pinComment: PropTypes.func,
	onEditComment: PropTypes.func,
	deleteComment: PropTypes.func,
	// actionHandlers: CommentActionHandlers
};

export default Comment;