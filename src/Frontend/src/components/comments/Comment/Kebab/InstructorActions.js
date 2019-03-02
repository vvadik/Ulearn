import React, { useContext } from "react";
import PropTypes from "prop-types";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import Icon from "@skbkontur/react-icons";

import styles from "./InstructorActions.less";

export default function InstructorActions(props) {
	const { commentActions, isApproved } = props;

	return <div className={styles.instructorsActions}>
		<Kebab positions={['bottom right']} size="large" disableAnimations={false}>
			<MenuItem
				icon={<Icon.Edit size="small"/>}
				onClick={commentActions.handleShowEditComment}>
				Редактировать
			</MenuItem>
			<MenuItem
				icon={<Icon.EyeClosed size="small"/>}
				onClick={commentActions.handleVisibleMark}>
				{ isApproved ? 'Опубликовать' : 'Скрыть' }
			</MenuItem>
			<MenuItem
				icon={<Icon.Delete size="small"/>}
				onClick={commentActions.handleDeleteComment}>
				Удалить
			</MenuItem>
		</Kebab>
	</div>
}

InstructorActions.propTypes = {
	isApproved: PropTypes.bool,
	commentActions: PropTypes.object,
};