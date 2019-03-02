import React from "react";
import Icon from "@skbkontur/react-icons";
// import { Link } from 'react-router-dom';

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
	const { user, author, userRoles, parentCommentId, url, hasReplyAction,
		canModerateComments, isCorrectAnswer, isPinnedToTop, commentActions } = props;

	const actions = [];

	if (!hasReplyAction) {
		actions.push(<Button onClick={commentActions.handleShowReplyForm} icon='ArrowCorner1'>Ответить</Button>);
	}

	if (user.id === author.id) {
		actions.push(<Button onClick={commentActions.handleShowEditComment} icon='Edit'>Редактировать</Button>);
		actions.push(<Button onClick={commentActions.handleDeleteComment} icon='Delete'>Удалить</Button>);
	}
	
	if (canModerateComments(userRoles, 'viewAllStudentsSubmissions')) {
		actions.push(<ActionLink url={url} icon='DocumentLite'>Посмотреть решения</ActionLink>);
	}

	if (canModerateComments(userRoles, 'editPinAndRemoveComments')) {
		if (parentCommentId) {
			actions.push(<Button onClick={commentActions.handleCorrectAnswerMark} icon={'Star2'}>
				{isCorrectAnswer ? 'Снять отметку' : 'Отметить правильным'}</Button>)
		} else {
			actions.push(<Button onClick={commentActions.handlePinnedToTopMark} icon={'Pin'}>
				{isPinnedToTop ? 'Открепить' : 'Закрепить'}</Button>)
		}
	}

	return (
		<div className={styles.actions}>
			{ actions }
			{/*{actions.map(action => {*/}
				{/*if (!action.isAvailable) {*/}
					{/*return null;*/}
				{/*}*/}

				{/*if (action.type === types.button) {*/}
					{/*return <Button >{action.text}</Button>*/}
				{/*}*/}
				{/*return <ActionLink icon={action}>{action.text}</ActionLink>*/}
			{/*})}*/}
		</div>
	)
};

