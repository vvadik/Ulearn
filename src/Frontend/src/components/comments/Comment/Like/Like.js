import React from "react";
import PropTypes from "prop-types";
import ThumbUp from "icons/ThumbUp";

import styles from "./Like.less";

export default function Like({isLiked, count, onClick, canLike}) {
	return (
		<div className={`${styles.wrapper} ${canLike ? styles.hover : ""} ${isLiked ? styles.isLiked : ""}`}>
			<button className={styles.action} onClick={canLike ? onClick : null}>
				<ThumbUp size={15} />
				<span className={styles.count}>{count}</span>
			</button>
		</div>
	)
}

Like.propTypes = {
	isLiked: PropTypes.bool,
	canLike: PropTypes.bool,
	count: PropTypes.number,
	onClick: PropTypes.func,
};