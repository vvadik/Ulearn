import React from "react";
import PropTypes from "prop-types";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import Icon from "@skbkontur/react-icons";

import styles from "./InstructorActions.less";

export default function InstructorActions({ isApproved, dispatch }) {
	const handleEditClick = () => dispatch('edit');
	const handleHideClick = () => dispatch('toggleHidden');
	const handleDeleteClick = () => dispatch('delete');

	return <div className={styles.instructorsActions}>
		<Kebab positions={['bottom right']} size="large" disableAnimations={false}>
			<MenuItem
				icon={<Icon.Edit size="small"/>}
				onClick={handleEditClick}>
				Редактировать
			</MenuItem>
			<MenuItem
				icon={<Icon.EyeClosed size="small"/>}
				onClick={handleHideClick}>
				{ isApproved ? 'Опубликовать' : 'Скрыть' }
			</MenuItem>
			<MenuItem
				icon={<Icon.Delete size="small"/>}
				onClick={handleDeleteClick}>
				Удалить
			</MenuItem>
		</Kebab>
	</div>
}

InstructorActions.propTypes = {
	isApproved: PropTypes.bool,
	dispatch: PropTypes.func,
};