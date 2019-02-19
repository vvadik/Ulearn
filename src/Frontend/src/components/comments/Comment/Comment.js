import React, {Component} from 'react';
import PropTypes from "prop-types";
import moment from "moment";
import * as debounce from "debounce";
import Icon from "@skbkontur/react-icons";
import Hint from "@skbkontur/react-ui/components/Hint/Hint";
import Textarea from "@skbkontur/react-ui/components/Textarea/Textarea";
import Avatar from "../../common/Avatar/Avatar";

import styles from "./comment.less";
import Button from "@skbkontur/react-ui/components/Button/Button";
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import MarkdownEditor from "../CommentSendForm/MarkdownEditor";
import Action from "./Action";

/*
Props:
  "is_approved": true,
  "is_correct_answer": true,
  "is_pinned_to_top": true,
  "likes_count": 0,
  "replies": [
    null
  ],
  "course_id": "string",
  "slide_id": "string",
  "reply_to": 0
}
*/

class Comment extends Component {
	constructor(props) {
		super(props);
		this.state = {
			commentId: props.commentId,
			likesCount: props.likesCount,
			isLiked: props.isLiked,
			openForm: false,
			openEditCommentForm: false,
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
									<Icon
										name={'ThumbUp'}
										color={isLiked ? '#D70C17' : '#A0A0A0'}
										size={16}/>
								</button>
								<span className={styles.likesCount}>{likesCount}</span>
							</div>
						</div>
						<Hint pos="bottom" text={publishDate}>
							<div className={styles.timeSinceAdded}>
								{moment(publishDate).fromNow()}
							</div>
						</Hint>
						{ this.state.openEditCommentForm ? this.renderEditCommentForm() :
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
					<span dangerouslySetInnerHTML={{__html: this.props.commentHtml}} />
				</p>
				{ this.renderActions() }
			</React.Fragment>
		)
	}

	renderEditCommentForm() {
		const { commentHtml, sending, commentId, author } = this.props;

		return (
			<CommentSendForm text={commentHtml} author={author} action={'edit'} sending={sending} />
		)
	}

	renderActions() {
		return (
			<div className={styles.actions}>
				<button type="button" className={styles.sendAnswer} onClick={this.openCommentSendForm}>
					<Icon name={'ArrowCorner1'} />
					<span className={styles.buttonText}>Ответить</span>
				</button>
				<button type="button" className={styles.sendAnswer} onClick={this.editComment}>
					<Icon name={'Edit'} />
					<span className={styles.buttonText}>Редактировать</span>
				</button>
				<button type="button" className={styles.sendAnswer} onClick={this.deleteComment}>
					<Icon name={'Delete'} />
					<span className={styles.buttonText}>Удалить</span>
				</button>
			</div>
		)
	}

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
	commentHtml: PropTypes.string,
	showReplyButton: PropTypes.bool,
	commentId: PropTypes.number,
};

export default Comment;