import React, { Component, createContext } from 'react';
import { CommentActionHandlers } from './commonPropTypes';
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

import styles from "./Comment.less";

export const CommentMarksContext = createContext({
	isApproved: false,
	isCorrectAnswer: false,
	isPinnedToTop: false,
});

class Comment extends Component {
	constructor(props) {
		super(props);
		this.state = {
			showEditForm: false,
			likesCount: props.likesCount,
			isLiked: props.isLiked,
			isCorrectAnswer: props.isCorrectAnswer,
			isApproved: props.isApproved,
			isPinnedToTop: props.isPinnedToTop,
		};

		this.debouncedSendData = debounce(this.sendData, 300);
	};

	render() {
		const {children, author, publishTime, userRoles } = this.props;
		const {likesCount, isLiked, isApproved, isCorrectAnswer, isPinnedToTop } = this.state;
		return (
			<CommentMarksContext.Provider value={{ isApproved, isCorrectAnswer, isPinnedToTop }}>
				<div className={styles.comment}>
					<Avatar user={author} size='big'/>
					<div className={styles.content}>
						<Header name={author.visibleName}>
							<Like checked={isLiked} count={likesCount} onClick={this.handleLikeClick} />
							<Marks
								isApproved={isApproved}
								isCorrectAnswer={isCorrectAnswer}
								isPinnedToTop={isPinnedToTop} />
							{ this.canModerateComments(userRoles, 'editPinAndRemoveComments') &&
							<InstructorActions isApproved={isApproved} dispatch={this.dispatch} actionHandlers={this.actionHandlers}/> }
						</Header>
						<Hint pos="bottom" text={publishTime} disableAnimations={false} useWrapper={false}>
							<div className={styles.timeSinceAdded}>
								{moment(publishTime).fromNow()}
							</div>
						</Hint>
						{ this.state.showEditForm ?	this.renderEditCommentForm() : this.renderComment() }
						{ children }
					</div>
				</div>
			</CommentMarksContext.Provider>
		);
	}

	renderComment() {
		return (
			<React.Fragment>
				<p className={styles.text}>
					<span dangerouslySetInnerHTML={{__html: this.props.renderCommentText}} />
				</p>
				<Actions
					{...this.props}
					{...this.state}
					dispatch={this.dispatch}
					canModerateComments={this.canModerateComments}/>
			</React.Fragment>
		)
	}

	renderEditCommentForm() {
		const { text, sending } = this.props;

		return (
			<CommentSendForm
				text={text}
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
		const {commentId, likeChanged} = this.props;

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


		this.debouncedSendData(likeChanged, commentId, isLiked);
	};

	showReplyForm = () => {
		this.props.showReplyForm();
	};

	showEditComment = () => {
		this.setState({
			showEditForm: true,
		});
	};

	handleEditCancel = () => {
		this.setState({ showEditForm: false });
	};

	handleEditComment = (text) => {
		const { commentId, onEditComment } = this.props;

		this.setState({
			showEditForm: false,
		});

		onEditComment(commentId, text);
	};

	markAsCorrectAnswer = () => {
		const { isCorrectAnswer } = this.state;
		const { commentId, markAsCorrectAnswer } = this.props;

		this.setState({
			isCorrectAnswer: !isCorrectAnswer,
		});

		this.debouncedSendData(markAsCorrectAnswer, commentId, isCorrectAnswer);
	};

	pinComment = () => {
		const { isPinnedToTop } = this.state;
		const { commentId, pinComment } = this.props;

		this.setState({
			isPinnedToTop: !isPinnedToTop,
		});

		this.debouncedSendData(pinComment, commentId, isPinnedToTop);
	};

	hideComment = () => {
		const { isApproved } = this.state;
		const { commentId, hideComment } = this.props;

		this.setState({
			isApproved: !isApproved,
		});

		this.debouncedSendData(hideComment, commentId, isApproved);
	};

	deleteComment = () => {
		this.props.deleteComment(this.props.commentId);
	};

	dispatch = (action) => {
		switch(action) {
			case 'togglePinned':
				return this.pinComment();
			case 'toggleCorrect':
				return this.markAsCorrectAnswer();
			case 'toggleHidden':
				return this.hideComment();
			case 'edit':
				return this.showEditComment();
			case 'delete':
				return this.deleteComment();
			case 'reply':
				return this.showReplyForm();
		}
	};

	sendData = (action, value, flag) => {
		return () => action(value, flag);
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
	commentId: PropTypes.number,
	author: Avatar.propTypes.user,
	user: Avatar.propTypes.user,
	userRoles: userRolesModel,
	text: PropTypes.string,
	renderCommentText: PropTypes.oneOfType([
		PropTypes.string, PropTypes.element
	]),
	url: PropTypes.string,
	publishTime: PropTypes.string,
	isLiked: PropTypes.bool,
	isApproved: PropTypes.bool,
	isPinnedToTop: PropTypes.bool,
	isCorrectAnswer: PropTypes.bool,
	parentCommentId: PropTypes.number,
	likesCount: PropTypes.number,
	likeChanged: PropTypes.func,
	showReplyForm: PropTypes.func,
	hideComment: PropTypes.func,
	onLikeChanged: PropTypes.func,
	markAsCorrectAnswer: PropTypes.func,
	pinComment: PropTypes.func,
	onSubmit: PropTypes.func,
	onEditComment: PropTypes.func,
	deleteComment: PropTypes.func,
	actionHandlers: CommentActionHandlers
};

export default Comment;