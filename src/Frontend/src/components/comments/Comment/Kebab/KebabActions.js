import React from "react";
import PropTypes from "prop-types";
import { comment, userType, userRoles } from "../../commonPropTypes";
import { Kebab, MenuItem } from "ui";
import { EyeClosed, Delete, Pin, Edit, DocumentLite, Ok } from "icons";
import { SlideType } from "src/models/slide";

import styles from "./KebabActions.less";
import { AccessType } from "src/consts/accessType";

export default function KebabActions(props) {
	const { user, comment, userRoles, url, canModerateComments, actions, slideType, handleCommentBackGround } = props;
	const canModerate = canModerateComments(userRoles, AccessType.editPinAndRemoveComments);
	const canDeleteAndEdit = (user.id === comment.author.id || canModerate);
	const canSeeSubmissions = (slideType === SlideType.Exercise &&
		canModerateComments(userRoles, AccessType.viewAllStudentsSubmissions));

	return (
		<div className={ styles.instructorsActions }>
			<Kebab positions={ ["bottom right"] } size="large" disableAnimations={ true }>
				{ canModerate &&
				<MenuItem
					icon={ <EyeClosed size="small"/> }
					onClick={ () => {
						actions.handleApprovedMark(comment.id, comment.isApproved);
						handleCommentBackGround(comment.id, comment.isApproved)
					}
					}>
					{ !comment.isApproved ? "Опубликовать" : "Скрыть" }
				</MenuItem> }
				{ canDeleteAndEdit &&
				<MenuItem
					icon={ <Delete size="small"/> }
					onClick={ () => actions.handleDeleteComment(comment) }>
					Удалить
				</MenuItem> }
				{ (canModerate && !comment.parentCommentId) &&
				<MenuItem
					onClick={ () => actions.handlePinnedToTopMark(comment.id, comment.isPinnedToTop) }
					icon={ <Pin size="small"/> }>
					{ comment.isPinnedToTop ? "Открепить" : "Закрепить" }
				</MenuItem> }
				{ canDeleteAndEdit &&
				<MenuItem
					icon={ <Edit size="small"/> }
					onClick={ () => actions.handleShowEditForm(comment.id) }>
					Редактировать
				</MenuItem> }
				{ canSeeSubmissions &&
				<div className={ styles.visibleOnPhone }>
					<MenuItem
						href={ url }
						icon={ <DocumentLite size="small"/> }>
						Посмотеть решения
					</MenuItem>
				</div> }
				{ (canModerate && comment.parentCommentId) &&
				<MenuItem
					onClick={ () => actions.handleCorrectAnswerMark(comment.id, comment.isCorrectAnswer) }
					icon={ <Ok size="small"/> }>
					{ comment.isCorrectAnswer ? "Снять отметку" : "Отметить правильным" }
				</MenuItem> }
			</Kebab>
		</div>
	)
}

KebabActions.propTypes = {
	comment: comment.isRequired,
	actions: PropTypes.objectOf(PropTypes.func),
	user: userType.isRequired,
	userRoles: userRoles.isRequired,
	url: PropTypes.string,
	canModerateComments: PropTypes.func,
	handleCommentBackGround: PropTypes.func,
	slideType: PropTypes.string,
};
