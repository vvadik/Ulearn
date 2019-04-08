import React from "react";
import PropTypes from "prop-types";
import { comment, userRoles, userType } from "../../commonPropTypes";
import Link from "@skbkontur/react-ui/components/Link/Link";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Icon from "@skbkontur/react-icons";
import { ACCESSES, SLIDETYPE } from "../../../../consts/general";

import styles from "./CommentActions.less";

const ButtonAction = ({onClick, icon, children}) => (
	<div className={styles.action}>
		<Button use="link" onClick={onClick} icon={<Icon name={icon} />}>
			{children}
		</Button>
	</div>
);

const LinkAction = ({url, icon, children}) => (
	<div className={styles.action}>
		<Link href={url} icon={<Icon name={icon} />}>
			{children}
		</Link>
	</div>
);

export default function CommentActions(props) {
	const {user, comment, userRoles, url, hasReplyAction, canModerateComments, actions, slideType} = props;

	const commentActions = [];

	if (hasReplyAction) {
		const commentId = comment.parentCommentId ? comment.parentCommentId : comment.id;

		commentActions.push(
			<ButtonAction
				key="Ответить"
				onClick={() => actions.handleShowReplyForm(commentId)}
				icon="ArrowCorner1">
				Ответить
			</ButtonAction>);
	}

	if (user.id === comment.author.id || canModerateComments(userRoles, ACCESSES.editPinAndRemoveComments)) {
		commentActions.push(
		<div className={styles.visibleOnDesktopAndTablet}  key="Редактировать">
			<ButtonAction
				onClick={() => actions.handleShowEditForm(comment.id)}
				icon="Edit">
				Редактировать
			</ButtonAction>
		</div>);
	}

	if (slideType === SLIDETYPE.exercise && canModerateComments(userRoles, ACCESSES.viewAllStudentsSubmissions)) {
		commentActions.push(
			<div className={styles.visibleOnDesktopAndTablet}  key="Решения">
				<LinkAction
					url={url}
					icon="DocumentLite">
					Посмотреть решения
				</LinkAction>
			</div>);
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

