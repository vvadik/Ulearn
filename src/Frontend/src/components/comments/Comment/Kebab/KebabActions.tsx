import React from "react";
import { Kebab, MenuItem } from "ui";
import { EyeClosed, Delete, Pin, Edit, DocumentLite, Ok } from "icons";

import { SlideType } from "src/models/slide";
import { Access, CourseAccessType } from "src/consts/accessType";
import { Comment } from "src/models/comments";
import { AccountState } from "src/redux/account";
import { UserRolesWithCourseAccesses } from "src/utils/courseRoles";
import { ActionsType } from "../../CommentsList/CommentsList";

import styles from "./KebabActions.less";


interface Props {
	url: string;
	slideType: SlideType;

	user: AccountState;
	userRoles: UserRolesWithCourseAccesses;

	comment: Comment;

	actions: ActionsType;
	canModerateComments: (userRoles: UserRolesWithCourseAccesses, access: CourseAccessType) => boolean;
	handleCommentBackGround: (commentId: string, isApproved: boolean) => void;
}

export default function KebabActions(props: Props): React.ReactElement {
	const { user, comment, userRoles, url, canModerateComments, actions, slideType, handleCommentBackGround } = props;
	const canModerate = canModerateComments(userRoles, Access.editPinAndRemoveComments);
	const canDeleteAndEdit = (user.id === comment.author.id || canModerate);
	const canSeeSubmissions = (slideType === SlideType.Exercise &&
		canModerateComments(userRoles, Access.viewAllStudentsSubmissions));

	return (
		<div className={ styles.instructorsActions }>
			<Kebab positions={ ["bottom right"] } size="large" disableAnimations={ true }>
				{ canModerate &&
				<MenuItem
					data-id={ comment.id }
					data-approved={ comment.isApproved }
					icon={ <EyeClosed size="small"/> }
					onClick={ handleApprovedMarkClick }>
					{ !comment.isApproved ? "Опубликовать" : "Скрыть" }
				</MenuItem> }
				{ canDeleteAndEdit &&
				<MenuItem
					icon={ <Delete size="small"/> }
					onClick={ handleDeleteCommentClick }>
					Удалить
				</MenuItem> }
				{ (canModerate && !comment.parentCommentId) &&
				<MenuItem
					onClick={ handlePinnedToTopMarkClick }
					icon={ <Pin size="small"/> }>
					{ comment.isPinnedToTop ? "Открепить" : "Закрепить" }
				</MenuItem> }
				{ canDeleteAndEdit &&
				<MenuItem
					icon={ <Edit size="small"/> }
					onClick={ handleShowEditFormClick }>
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
					onClick={ handleCorrectAnswerMarkClick }
					icon={ <Ok size="small"/> }>
					{ comment.isCorrectAnswer ? "Снять отметку" : "Отметить правильным" }
				</MenuItem> }
			</Kebab>
		</div>
	);

	function handleApprovedMarkClick(): void {
		actions.handleApprovedMark(comment.id, comment.isApproved);
		handleCommentBackGround(comment.id, comment.isApproved);
	}

	function handleDeleteCommentClick() {
		actions.handleDeleteComment(comment);
	}

	function handlePinnedToTopMarkClick() {
		actions.handlePinnedToTopMark(comment.id, comment.isPinnedToTop);
	}

	function handleShowEditFormClick() {
		actions.handleShowEditForm(comment.id);
	}

	function handleCorrectAnswerMarkClick() {
		actions.handleCorrectAnswerMark(comment.id, comment.isCorrectAnswer);
	}
}
