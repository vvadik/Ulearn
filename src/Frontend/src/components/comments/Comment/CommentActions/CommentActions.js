import React from "react";
import PropTypes from "prop-types";
import { comment, userRoles, userType } from "../../commonPropTypes";
import uuid from 'uuid';
import Link from "@skbkontur/react-ui/components/Link/Link";
import Icon from "@skbkontur/react-icons";
import { NotMobile } from "../../../../utils/responsive";

import styles from "../Comment.less";

const Button = ({onClick, icon, children}) => (
	<button type="button" className={styles.sendAnswer} onClick={onClick}>
		<Icon name={icon} />
		<span className={styles.buttonText}>{children}</span>
	</button>
);

const ActionLink = ({url, icon, children}) => (
	<Link href={url}>
		<Icon name={icon} />
		<span className={styles.linkText}>{children}</span>
	</Link>
);

export default function CommentActions(props) {
	const {user, comment, userRoles, url, hasReplyAction, canModerateComments, actions, slideType} = props;

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

	if (user.id === comment.author.id || canModerateComments(userRoles, 'editPinAndRemoveComments')) {
		commentActions.push(
			<NotMobile key={uuid()}>
				<Button
					onClick={() => actions.handleShowEditForm(comment.id)}
					icon='Edit'>
				Редактировать
				</Button>
			</NotMobile>);
	}

	if (slideType === 'exercise' && canModerateComments(userRoles, 'viewAllStudentsSubmissions')) {
		commentActions.push(
			<NotMobile key={uuid()}>
				<ActionLink
					url={url}
					icon='DocumentLite'>
				Посмотреть решения
				</ActionLink>
			</NotMobile>);
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
	slideType: PropTypes.string,
	getUserSolutionsUrl: PropTypes.func,
};

