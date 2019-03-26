import React from "react";
import PropTypes from "prop-types";
import { comment, userType, userRoles } from "../../commonPropTypes";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import Icon from "@skbkontur/react-icons";
import { Mobile } from "../../../../utils/responsive";

import styles from "./KebabActions.less";

export default function KebabActions(props) {
	const {user, comment, userRoles, url, canModerateComments, actions, slideType} = props;

		return (
			<div className={styles.instructorsActions}>
		<Kebab positions={['left top']} size="large" disableAnimations={false}>
			{(user.id === comment.author.id || canModerateComments(userRoles, 'editPinAndRemoveComments')) &&
				<>
					<MenuItem
						icon={<Icon.Delete size="small" />}
						onClick={() => actions.handleDeleteComment(comment.id)}>
						Удалить
					</MenuItem>
					<Mobile>
						<MenuItem
							icon={<Icon.Edit size="small" />}
							onClick={() => actions.handleShowEditForm(comment.id)}>
							Редактировать
						</MenuItem>
					</Mobile>
				</>}
			{(slideType === 'exercise' && canModerateComments(userRoles, 'viewAllStudentsSubmissions')) &&
				<Mobile>
					<MenuItem
						href={url}
						icon={<Icon name='DocumentLite' size="small" />}>
						Посмотеть решения
					</MenuItem>
				</Mobile>}
			{canModerateComments(userRoles, 'editPinAndRemoveComments') ?
				<>
				<MenuItem
					icon={<Icon.EyeClosed size="small" />}
					onClick={() => actions.handleApprovedMark(comment.id, comment.isApproved)}>
					{!comment.isApproved ? 'Опубликовать' : 'Скрыть'}
				</MenuItem>
				{comment.parentCommentId ?
				<MenuItem
					onClick={() => actions.handleCorrectAnswerMark(comment.id, comment.isCorrectAnswer)}
					icon={<Icon name='Ok' size="small" />}>
					{comment.isCorrectAnswer ? 'Снять отметку' : 'Отметить правильным'}
				</MenuItem> :
				<MenuItem
					onClick={() => actions.handlePinnedToTopMark(comment.id, comment.isPinnedToTop)}
					icon={<Icon name='Pin' size="small" />}>
					{comment.isPinnedToTop ? 'Открепить' : 'Закрепить'}
				</MenuItem>}
				</>
			: null}
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
	slideType: PropTypes.string,
};