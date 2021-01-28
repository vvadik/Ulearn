import React from "react"
import PropTypes from "prop-types";

import { Link } from "ui";

import { courseStatistics } from "src/consts/routes";
import { buildQuery } from "src/utils";
import { groupAsStudentType } from "../types";

import styles from './linkToGroupsStatements.less';

export default function LinksToGroupsStatements({ groupsAsStudent }) {
	const groupsLinks = [];

	for (let i = 0; i < groupsAsStudent.length; i++) {
		let { id, courseId, name, } = groupsAsStudent[i];
		courseId = courseId.toLowerCase();

		groupsLinks.push(
			<Link
				key={ id }
				href={ courseStatistics + buildQuery({ courseId, group: id }) }>
				{ name }
			</Link>
		);

		if (i < groupsAsStudent.length - 1) {
			groupsLinks.push(', ');
		}
	}

	return <p className={ styles.root }>Ведомость { groupsLinks }</p>;
}

LinksToGroupsStatements.propTypes = {
	groupsAsStudent: PropTypes.arrayOf(PropTypes.shape(groupAsStudentType))
};
