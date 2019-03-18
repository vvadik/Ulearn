import React from "react";
import PropTypes from "prop-types";
import Link from "@skbkontur/react-ui/components/Link/Link";

import styles from "./Header.less";

export default function Header(props) {
	const {name, children, canViewProfiles, profileUrl} = props;
	return (
		<div className={styles.header}>
			{canViewProfiles ? <Link href={profileUrl}><h3 className={styles.author}>{name}</h3></Link> :
				<h3 className={styles.author}>{name}</h3>}
			{children}
		</div>
	);
}

Header.propTypes = {
	name: PropTypes.string,
	children: PropTypes.array,
	canViewProfile: PropTypes.bool,
	profileUrl: PropTypes.string,
};
