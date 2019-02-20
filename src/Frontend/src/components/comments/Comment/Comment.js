import React, {Component} from 'react';
import PropTypes from "prop-types";
import moment from "moment";
import * as debounce from "debounce";
import Icon from "@skbkontur/react-icons";
import Hint from "@skbkontur/react-ui/components/Hint/Hint";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import Avatar from "../../common/Avatar/Avatar";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import CommentSendForm from "../CommentSendForm/CommentSendForm";

import styles from "./comment.less";

class Comment extends Component {
	constructor(props) {
		super(props);
		this.state = {
			likesCount: props.likesCount,
			isLiked: props.isLiked,
			openForm: false,
			openEditCommentForm: false,
			isCorrectAnswer: false,
			isApproved: false,
			isPinnedToTop: false,
		};
		this.debouncedSendChangedLike = debounce(this.sendChangedLike, 300);
	};

	render() {
		const {children, author, publishDate} = this.props;
		const {likesCount, isLiked} = this.state;
		return (
			<React.Fragment>
				<div className={styles.comment}>
					<Avatar user={author} size='big'/>
					<div className={styles.content}>
						<div className={styles.header}>
							<h3 className={styles.author}>{author.visibleName}</h3>
							<div className={styles.likesBlock}>
								<button className={styles.likesAction} onClick={this.onLikeChanged}>
									<Icon name='ThumbUp' color={isLiked ? '#D70C17' : '#A0A0A0'} size={16} />
								</button>
								<span className={styles.likesCount}>{likesCount}</span>
							</div>
							{ this.state.isApproved && this.renderHiddenCommentMark() }
							{ this.state.isCorrectAnswer && this.renderCorrectAnswerMark() }
							{ this.state.isPinnedToTop && this.renderPinnedToTopMark() }
							<div className={styles.instructorsActions}>
								<Kebab positions={['bottom right']} size="large">
									<MenuItem icon={<Icon.Edit size="small"/>} onClick={this.editComment}>Редактировать</MenuItem>
									<MenuItem icon={<Icon.EyeClosed size="small"/>} onClick={this.hideComment}>{ this.state.isApproved ? 'Опубликовать' : 'Скрыть' }</MenuItem>
									<MenuItem icon={<Icon.Delete size="small"/>} onClick={this.deleteComment}>Удалить</MenuItem>
								</Kebab>
						</div>
						</div>
						<Hint pos="bottom" text={publishDate}>
							<div className={styles.timeSinceAdded}>
								{moment(publishDate).fromNow()}
							</div>
						</Hint>
						{ this.state.openEditCommentForm ?
							this.renderEditCommentForm() :
							this.renderComment()}
						{ this.state.openForm && children }
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
				onSubmit={(text) => console.log(`Saving '${text}' to api`)}
				submitTitle={'Сохранить'}
				onCancel={this.handleEditCancel}
				sending={sending} />
		)
	}

	renderActions() {
		return (
			<div className={styles.actions}>
				{this.renderButton(this.openCommentSendForm, 'ArrowCorner1', 'Ответить')}
				{this.renderButton(this.editComment, 'Edit', 'Редактировать')}
				{this.renderButton(this.deleteComment, 'Delete', 'Удалить')}
				{this.renderButton(this.seeSolutions, 'DocumentLite', 'Посмотреть решения')}
				{this.renderButton(this.markAsCorrectAnswer, 'Star2', this.state.isCorrectAnswer ? 'Снять отметку' : 'Отметить правильным')}
				{this.renderButton(this.pinnedComment, 'Pin', this.state.isPinnedToTop ? 'Открепить' : 'Закрепить')}
			</div>
		)
	}

	renderButton(onClick, icon, text) {
		return <button type="button" className={styles.sendAnswer} onClick={onClick}>
			<Icon name={icon} />
			<span className={styles.buttonText}>{text}</span>
		</button>;
	}

	handleEditCancel = () => {
		this.setState({ openEditCommentForm: false });
	};

	onLikeChanged = () => {
		const {isLiked, likesCount} = this.state;

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

		this.debouncedSendChangedLike(isLiked, likesCount);
	};

	sendChangedLike = (isLiked, likesCount) => {
		return () => this.props.likeChanged(isLiked, likesCount);
	};

	openCommentSendForm = () => {
		this.setState({
			openForm: true,
		});
	};

	deleteComment = () => {
		const { commentId } = this.props;

		this.props.deleteComment(commentId);
	};

	editComment = () => {
		this.setState({
			openEditCommentForm: true,
		});
	};

	markAsCorrectAnswer = () => {
		const { isCorrectAnswer } = this.state;

		this.setState({
			isCorrectAnswer: !isCorrectAnswer,
		});

		this.props.markAsCorrectAnswer(isCorrectAnswer);
	};

	pinnedComment = () => {
		const { isPinnedToTop } = this.state;
		const { commentId } = this.props;

		this.setState({
			isPinnedToTop: !isPinnedToTop,
		});

		this.props.pinnedComment(commentId, isPinnedToTop);
	};

	hideComment = () => {
		const { isApproved } = this.state;
		this.setState({
			isApproved: !isApproved,
		});

		this.props.hideComment(isApproved);
	}
}

const accountModel = PropTypes.shape({
	id: PropTypes.string,
	url: PropTypes.string,
});

Comment.propTypes = {
	/** Идентифицирует комментарий, с которым работает компонент.
	 * При изменении идентификатора текст в поле ввода очищается. При сохранении того же идентификатора - текст сохраняется. */
	isLiked: PropTypes.bool,
	likesCount: PropTypes.number,
	likeChanged: PropTypes.func,
	sendFormOpened: PropTypes.bool,
	author: accountModel,
	publishDate: PropTypes.string,
	commentText: PropTypes.string,
	showReplyButton: PropTypes.bool,
	commentId: PropTypes.number,
	hideComment: PropTypes.func,
	markAsCorrectAnswer: PropTypes.func,
	pinnedComment: PropTypes.func,
};

export default Comment;