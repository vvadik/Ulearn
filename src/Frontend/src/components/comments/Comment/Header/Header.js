import React from "react";
import PropTypes from "prop-types";

import styles from "./Header.less";

export default function Header({ name, children }) {
	return (
		<div className={styles.header}>
			<h3 className={styles.author}>{name}</h3>
			{children}
		</div>
	);
}

Header.propTypes = {
	name: PropTypes.string,
};
