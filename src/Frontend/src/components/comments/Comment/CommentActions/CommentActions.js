import React from "react";
import PropTypes from "prop-types";
import { comment, userRoles, userType } from "../../commonPropTypes";
import uuid from 'uuid';
import Link from "@skbkontur/react-ui/components/Link/Link";
import Icon from "@skbkontur/react-icons";
import { Mobile, NotMobile } from "../../../../utils/responsive";

import styles from "../Comment.less";

const Button = ({onClick, icon, children}) => (
	<button type="button" className={styles.sendAnswer} onClick={onClick}>
		<Icon name={icon} />
		<span className={styles.buttonText}><NotMobile>{children}</NotMobile></span>
	</button>
);

const ActionLink = ({url, icon, children}) => (
	<Link href={url}>
		<Icon name={icon} />
		<span className={styles.linkText}><NotMobile>{children}</NotMobile></span>
	</Link>
);

export default function CommentActions(props) {
	const {user, comment, userRoles, url, hasReplyAction, canModerateComments, actions} = props;

	const commentActions = [];

	if (hasReplyAction) {
		const commentId = comment.parentCommentId ? comment.parentCommentId : comment.id;

		commentActions.push(
			<Button
				key={uuid()}
				onClick={() => actions.handleShowReplyForm(commentId)}
				icon='ArrowCorner1'>
			Ответить
			</Button>);
	}

	if (user.id === comment.author.id) {
		commentActions.push(
			<Button
				key={uuid()}
				onClick={() => actions.handleShowEditForm(comment.id)}
				icon='Edit'>
			Редактировать
			</Button>);

		commentActions.push(
			<Button
				key={uuid()}
				onClick={() => actions.handleDeleteComment(comment.id)}
				icon='Delete'>
			Удалить
			</Button>);
	}

	if (canModerateComments(userRoles, 'viewAllStudentsSubmissions')) {
		commentActions.push(
			<ActionLink
				key={uuid()}
				url={url}
				icon='DocumentLite'>
			Посмотреть решения
			</ActionLink>);
	}

	if (canModerateComments(userRoles, 'editPinAndRemoveComments')) {
		if (comment.parentCommentId) {
			commentActions.push(
				<Button
					key={uuid()}
					onClick={() => actions.handleCorrectAnswerMark(comment.id, comment.isCorrectAnswer)}
					icon={'Star2'}>
				{comment.isCorrectAnswer ? 'Снять отметку' : 'Отметить правильным'}
				</Button>)
		} else {
			commentActions.push(
				<Button
					key={uuid()}
					onClick={() => actions.handlePinnedToTopMark(comment.id, comment.isPinnedToTop)}
					icon={'Pin'}>
				{comment.isPinnedToTop ? 'Открепить' : 'Закрепить'}
				</Button>)
		}
	}

	return (
		<div className={styles.actions}>
			{commentActions}
		</div>
	)
};

CommentActions.propTypes = {
	user: userType.isRequired,
	userRoles: userRoles.isRequired,
	comment: comment.isRequired,
	actions: PropTypes.objectOf(PropTypes.func),
	url: PropTypes.string,
	hasReplyAction: PropTypes.bool,
	canModerateComments: PropTypes.func,
};

