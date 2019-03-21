import React from "react";
import PropTypes from "prop-types";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import Icon from "@skbkontur/react-icons";

import styles from "./InstructorActions.less";

export default function InstructorActions(props) {
	const {actions, isApproved, commentId} = props;

	return <div className={styles.instructorsActions}>
		<Kebab positions={['bottom right']} size="large" disableAnimations={false}>
			<MenuItem
				icon={<Icon.Edit size="small" />}
				onClick={() => actions.handleShowEditForm(commentId)}>
				Редактировать
			</MenuItem>
			<MenuItem
				icon={<Icon.EyeClosed size="small" />}
				onClick={() => actions.handleApprovedMark(commentId, isApproved)}>
				{!isApproved ? 'Опубликовать' : 'Скрыть'}
			</MenuItem>
			<MenuItem
				icon={<Icon.Delete size="small" />}
				onClick={() => actions.handleDeleteComment(commentId)}>
				Удалить
			</MenuItem>
		</Kebab>
	</div>
}

InstructorActions.propTypes = {
	commentId: PropTypes.number,
	actions: PropTypes.objectOf(PropTypes.func),
	isApproved: PropTypes.bool,
};