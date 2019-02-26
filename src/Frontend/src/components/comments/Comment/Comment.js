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
			replies: props.replies,
			likesCount: props.likesCount,
			isLiked: props.isLiked,
			showReplyForm: false,
			showEditForm: false,
			isCorrectAnswer: false,
			isApproved: false,
			isPinnedToTop: false,
		};
		this.debouncedSendData = debounce(this.sendData, 300);
	};

	render() {
		const {children, author, publishDate, userRoles } = this.props;
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
							<InstructorActions isApproved={isApproved} dispatch={this.dispatch}/> }
						</Header>
						<Hint pos="bottom" text={publishDate} disableAnimations={false} useWrapper={false}>
							<div className={styles.timeSinceAdded}>
								{moment(publishDate).fromNow()}
							</div>
						</Hint>
						{ this.state.showEditForm ?	this.renderEditCommentForm() : this.renderComment() }
						{ this.state.showReplyForm && children }
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
				onSubmit={this.onSubmit}
				submitTitle={'Сохранить'}
				onCancel={this.handleEditCancel}
				sending={sending} />
		)
	}

	canModerateComments = (role, accesses) => {
		return role.isSystemAdministrator || role.courseRole === 'CourseAdmin' ||
			(role.courseRole === 'Instructor' && role.courseAccesses.includes(accesses)) };

	handleEditCancel = () => {
		this.setState({ showEditForm: false });
	};

	handleLikeClick = () => {
		const {isLiked, likesCount} = this.state;
		const { likeChanged } = this.props;

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

		this.debouncedSendData(likeChanged, likesCount, isLiked);
	};

	showReplyForm = () => {
		this.setState({
			showReplyForm: true,
		});
	};

	deleteComment = () => {
		const { commentId } = this.props;

		this.props.deleteComment(commentId);
	};

	showEditComment = () => {
		this.setState({
			showEditForm: true,
		});
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

	hideComment = () => {
		const { isApproved } = this.state;
		const { commentId, hideComment } = this.props;

		this.setState({
			isApproved: !isApproved,
		});

		this.debouncedSendData(hideComment, commentId, isApproved);
	};

	sendData = (action, value, flag) => {
		return () => action(value, flag);
	};

	onSubmit = (text) => {
		const { commentId } = this.props;

		this.setState({
			showReplyForm: false,
			showEditForm: false,
		});

		this.props.onSubmit(commentId, text);
	}
}

const authorModel = PropTypes.shape({
	id: PropTypes.string.isRequired,
	visibleName: PropTypes.string.isRequired,
	avatarUrl: PropTypes.string,
});

const userModel = PropTypes.shape({
	id: PropTypes.string.isRequired,
	visibleName: PropTypes.string.isRequired,
	avatarUrl: PropTypes.string,
});

const userRolesModel = PropTypes.shape({
	isSystemAdministrator: PropTypes.bool,
	courseRole: PropTypes.string.isRequired,
	courseAccesses: PropTypes.arrayOf(PropTypes.string),
});

Comment.propTypes = {
	/** Идентифицирует комментарий, с которым работает компонент.
	 * При изменении идентификатора текст в поле ввода очищается. При сохранении того же идентификатора - текст сохраняется. */
	user: userModel,
	userRoles: userRolesModel,
	author: authorModel,
	replies: PropTypes.arrayOf(PropTypes.object),
	parentCommentId: PropTypes.number,
	isLiked: PropTypes.bool,
	isApproved: PropTypes.bool,
	likesCount: PropTypes.number,
	likeChanged: PropTypes.func,
	sendFormOpened: PropTypes.bool,
	publishDate: PropTypes.string,
	text: PropTypes.string,
	renderCommentText: PropTypes.oneOfType([
		PropTypes.string, PropTypes.element
	]),
	showReplyButton: PropTypes.bool,
	commentId: PropTypes.number,
	hideComment: PropTypes.func,
	markAsCorrectAnswer: PropTypes.func,
	pinComment: PropTypes.func,
	url: PropTypes.string,
	onSubmit: PropTypes.func,
	onChange: PropTypes.func,
	deleteComment: PropTypes.func,
};

export default Comment;