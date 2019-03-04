import React from "react";
import Icon from "@skbkontur/react-icons";

import styles from "../Comment.less";

const Button = ({ onClick, icon, children }) => (
	<button type="button" className={styles.sendAnswer} onClick={onClick}>
		<Icon name={icon} />
		<span className={styles.buttonText}>{children}</span>
	</button>
);

const ActionLink = ({ url, icon, children }) => (
	<a href={url} className={styles.sendAnswer}>
		<Icon name={icon} />
		<span className={styles.linkText}>{children}</span>
	</a>
);

export default function CommentActions(props) {
	const { user, comment, userRoles, url, hasReplyAction, canModerateComments,
		handleShowEditForm, handleShowReplyForm, actions } = props;

	const commentActions = [];

	if (!hasReplyAction) {
		commentActions.push(<Button onClick={() => handleShowReplyForm(true)} icon='ArrowCorner1'>Ответить</Button>);
	}

	if (user.id === comment.author.id) {
		commentActions.push(<Button onClick={() => handleShowEditForm(true)} icon='Edit'>Редактировать</Button>);
		commentActions.push(<Button onClick={() => actions.handleDeleteComment(comment.id)} icon='Delete'>Удалить</Button>);
	}
	
	if (canModerateComments(userRoles, 'viewAllStudentsSubmissions')) {
		commentActions.push(<ActionLink url={url} icon='DocumentLite'>Посмотреть решения</ActionLink>);
	}

	if (canModerateComments(userRoles, 'editPinAndRemoveComments')) {
		if (comment.parentCommentId) {
			commentActions.push(<Button onClick={() => actions.handleCorrectAnswerMark(comment.id)} icon={'Star2'}>
				{comment.isCorrectAnswer ? 'Снять отметку' : 'Отметить правильным'}</Button>)
		} else {
			commentActions.push(<Button onClick={() => actions.handlePinnedToTopMark(comment.id)} icon={'Pin'}>
				{comment.isPinnedToTop ? 'Открепить' : 'Закрепить'}</Button>)
		}
	}

	return (
		<div className={styles.actions}>
			{ commentActions }
		</div>
	)
};

