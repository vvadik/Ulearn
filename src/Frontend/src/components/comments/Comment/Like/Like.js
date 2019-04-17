import React from "react";
import PropTypes from "prop-types";
import Icon from "@skbkontur/react-icons";

import styles from "./Like.less";

export default function Like({isLiked, count, onClick, canLike}) {
	return (
		<div className={`${styles.wrapper} ${canLike ? styles.hover : ' '}`}>
			<button className={styles.action} onClick={canLike ? onClick : null}>
				<Icon name="ThumbUp" color={isLiked ? "#D70C17" : "#A0A0A0"} size={16} />
			</button>
			<span className={styles.count}>{count}</span>
		</div>
	)
}

Like.propTypes = {
	isLiked: PropTypes.bool,
	canLike: PropTypes.bool,
	count: PropTypes.number,
	onClick: PropTypes.func,
};