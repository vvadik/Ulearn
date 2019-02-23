import React, {Component} from 'react';
import PropTypes from "prop-types";
import moment from "moment";
import * as debounce from "debounce";
import Icon from "@skbkontur/react-icons";
import Hint from "@skbkontur/react-ui/components/Hint/Hint";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import Avatar from "../../common/Avatar/Avatar";
import CommentSendForm from "../CommentSendForm/CommentSendForm";

import styles from "./comment.less";



class Comment extends Component {
	constructor(props) {
		super(props);
		this.state = {
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
		const {children, author, publishDate, userRoles} = this.props;
		const {likesCount, isLiked} = this.state;
		return (
			<React.Fragment>
				<div className={styles.comment}>
					<Avatar user={author} size='big'/>
					<div className={styles.content}>
						<div className={styles.header}>
							<h3 className={styles.author}>{author.visibleName}</h3>
							<div>
								<button className={styles.likesAction} onClick={this.onLikeChanged}>
									<Icon name='ThumbUp' color={isLiked ? '#D70C17' : '#A0A0A0'} size={16} />
								</button>
								<span className={styles.likesCount}>{likesCount}</span>
							</div>
							{ this.state.isApproved && this.renderHiddenCommentMark() }
							{ this.state.isCorrectAnswer && this.renderCorrectAnswerMark() }
							{ this.state.isPinnedToTop && this.renderPinnedToTopMark() }
							{ this.canModerateComments(userRoles, 'editPinAndRemoveComments') &&
								<div className={styles.instructorsActions}>
									<Kebab positions={['bottom right']} size="large" disableAnimations={false}>
										<MenuItem
											icon={<Icon.Edit size="small"/>}
											onClick={this.editComment}>
											Редактировать
										</MenuItem>
										<MenuItem
											icon={<Icon.EyeClosed size="small"/>}
											onClick={this.hideComment}>
											{ this.state.isApproved ? 'Опубликовать' : 'Скрыть' }
										</MenuItem>
										<MenuItem
											icon={<Icon.Delete size="small"/>}
											onClick={this.deleteComment}>
											Удалить
										</MenuItem>
									</Kebab>
								</div>
							}
						</div>
						<Hint pos="bottom" text={publishDate} disableAnimations={false} useWrapper={false}>
							<div className={styles.timeSinceAdded}>
								{moment(publishDate).fromNow()}
							</div>
						</Hint>
						{ this.state.showEditForm ?
							this.renderEditCommentForm() :
							this.renderComment()}
						{ this.state.showReplyForm && children }
					</div>
				</div>
			</React.Fragment>
		);
	}

	renderComment() {
		return (
			<React.Fragment>
				<p className={styles.text}>
					<span dangerouslySetInnerHTML={{__html: this.props.commentText}} />
				</p>
				{ this.renderActions() }
			</React.Fragment>
		)
	}

	renderCorrectAnswerMark() {
		return (
		<div className={styles.correctAnswerMark}>
			<Icon name='Star' size={15} />
			<span className={styles.correctAnswerText}>Правильный ответ</span>
		</div>
		)
	}

	renderHiddenCommentMark() {
		return (
			<div className={styles.hiddenCommentMark}>
				Скрытый
			</div>
		)
	}

	renderPinnedToTopMark() {
		return (
			<div className={styles.pinnedToTopMark}>
				<Icon name='Pin' size={15} />
				<span className={styles.pinnedToTopText}>Закреплено</span>
			</div>
		)
	}

	renderEditCommentForm() {
		const { commentText, sending } = this.props;

		return (
			<CommentSendForm
				text={commentText}
				autofocus
				onSubmit={this.onSubmit}
				submitTitle={'Сохранить'}
				onCancel={this.handleEditCancel}
				sending={sending} />
		)
	}

	renderActions() {
			const { user, author, userRoles, parentCommentId } = this.props;

		return (
			<div className={styles.actions}>
				{ !parentCommentId && this.renderButton(this.showReplyForm, 'ArrowCorner1', 'Ответить') }
				{ user.id === author.id &&
					<div>
						{this.renderButton(this.editComment, 'Edit', 'Редактировать')}
						{this.renderButton(this.deleteComment, 'Delete', 'Удалить')}
					</div>
				}
				{ this.canModerateComments(userRoles, 'viewAllStudentsSubmissions') && this.renderLink() }
				{ this.canModerateComments(userRoles, 'editPinAndRemoveComments') &&
					<div>
						{ parentCommentId ? this.renderButton(this.markAsCorrectAnswer, 'Star2',
							this.state.isCorrectAnswer ? 'Снять отметку' : 'Отметить правильным') :
						 this.renderButton(this.pinComment, 'Pin',
							this.state.isPinnedToTop ? 'Открепить' : 'Закрепить') }
					</div>
				}
			</div>
		)
	}

	renderButton(onClick, icon, text) {
		return <button type="button" className={styles.sendAnswer} onClick={onClick}>
			<Icon name={icon} />
			<span className={styles.buttonText}>{text}</span>
		</button>;
	}

	renderLink() {
		return (
			<a href={this.props.url} className={styles.sendAnswer}>
				<Icon name='DocumentLite' />
				<span className={styles.linkText}>Посмотреть решения</span>
			</a>
		)
	}

	canModerateComments = (role, accesses) => {
		return role.isSystemAdministrator || role.courseRole === 'CourseAdmin' ||
			(role.courseRole === 'Instructor' && role.courseAccesses.includes(accesses)) };

	handleEditCancel = () => {
		this.setState({ showEditForm: false });
	};

	onLikeChanged = () => {
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

	editComment = () => {
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

	onSubmit = (e, text) => {
		e.preventDefault();

		this.props.onSubmit(text);
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
	parentCommentId: PropTypes.number,
	isLiked: PropTypes.bool,
	isApproved: PropTypes.bool,
	likesCount: PropTypes.number,
	likeChanged: PropTypes.func,
	sendFormOpened: PropTypes.bool,
	publishDate: PropTypes.string,
	commentText: PropTypes.string,
	showReplyButton: PropTypes.bool,
	commentId: PropTypes.number,
	hideComment: PropTypes.func,
	markAsCorrectAnswer: PropTypes.func,
	pinComment: PropTypes.func,
	url: PropTypes.string,
	onSubmit: PropTypes.func,
};

export default Comment;