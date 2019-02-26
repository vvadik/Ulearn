import React from "react";
import PropTypes from "prop-types";
import Icon from "@skbkontur/react-icons";

import styles from "./Like.less";

export default function Like({ checked, count, onClick }) {
	return <div>
		<button className={styles.action} onClick={onClick}>
			<Icon name='ThumbUp' color={checked ? '#D70C17' : '#A0A0A0'} size={16} />
		</button>
		<span className={styles.count}>{count}</span>
	</div>;
}

Like.propTypes = {
	checked: PropTypes.bool,
	count: PropTypes.number,
	onClick: PropTypes.func,
};