import React, {Component} from 'react';
import PropTypes from "prop-types";
import Icon from "@skbkontur/react-icons";
import Avatar from "../../common/Avatar/Avatar";

import styles from "./comment.less";
import moment from "moment";
import * as debounce from "debounce";

/*
Props:
isLiked Залайкано ли текущим пользователем?
likesCount Колиество лойков
Событие likeChanged(поставил ли лайк?) debounce
isTopLevel
bool sendFormOpened
bool showReplyButton

State:
	likesCount из пропсов
	isLiked из пропсов

 */

class Comment extends Component {
	constructor(props) {
		super(props);
		this.state = {
			openForm: props.sendFormOpened,
			likesCount: props.likesCount,
			isLiked: props.isLiked,
		};

		this.debouncedSendChangedLike = debounce(this.sendChangedLike, 300);
	};

	// static getDerivedStateFromProps(props, state) {
	// 	if (props.likesCount !== state.likesCount) {
	// 		return {
	// 			commentId: props.likesCount,
	// 		}
	// 	}
	//
	// 	return null;
	// }

	render() {
		const {children, author, commentText, publishDate, showReplyButton } = this.props;
		const { likesCount, isLiked } = this.state;
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
										size={16} />
								</button>
								<span className={styles.likesCount}>{likesCount}</span>
							</div>
						</div>
						<div className={styles.timeSinceAdded}>{moment(publishDate).fromNow()}</div>
						<p className={styles.text}>{ commentText }</p>
						<div className={styles.actions}>
							{ showReplyButton &&
							<button className={styles.sendAnswer} onClick={this.openCommentSendForm}>
								<Icon name={'ArrowCorner1'} color={'#3072C4'}/>
								<span className={styles.buttonText}>Ответить</span>
							</button>}
							<button className={`${styles.sendAnswer} ${styles.userButton}`}>
								<Icon name={'Edit'} color={'#3072C4'}/>
								<span className={styles.buttonText}>Редактировать</span>
							</button>
							<button className={`${styles.sendAnswer} ${styles.userButton}`}>
								<Icon name={'Delete'} color={'#3072C4'}/>
								<span className={styles.buttonText}>Удалить</span>
							</button>
						</div>
						{ this.state.openForm && children }
					</div>
				</div>
			</React.Fragment>
		);
	}

	onLikeChanged = () => {
		const { isLiked, likesCount } = this.state;

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

	onSubmit = (event) => {
		const {onSubmit} = this.props;

		event.preventDefault();

		// if (!text) {
		// 	this.setState({
		// 		error: "Заполните поле комментария",
		// 	});
		// 	return;
		// }

		onSubmit();
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
	sendFormOpened: PropTypes.bool,
	likesCount: PropTypes.number,
	likeChanged: PropTypes.func,
	publishDate: PropTypes.string,
	showReplyButton: PropTypes.bool,
	commentId: PropTypes.string,
	author: accountModel,
	sending: PropTypes.bool,
	error: PropTypes.oneOf([PropTypes.string, PropTypes.object]),
	onSubmit: PropTypes.func,
};

export default Comment;