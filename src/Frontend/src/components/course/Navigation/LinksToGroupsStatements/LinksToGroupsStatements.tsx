import React from "react";

import { Link } from "ui";

import { buildQuery } from "src/utils";

import { courseStatistics } from "src/consts/routes";
import { GroupAsStudentInfo } from "src/models/groups";

import styles from './linkToGroupsStatements.less';


interface Props {
	groupsAsStudent: GroupAsStudentInfo[];
}

export default function LinksToGroupsStatements({ groupsAsStudent }: Props): React.ReactElement {
	const groupsLinks = [];

	for (let i = 0; i < groupsAsStudent.length; i++) {
		const { id, courseId, name, } = groupsAsStudent[i];
		const courseIdInLowerCase = courseId.toLowerCase();

		groupsLinks.push(
			<Link
				key={ id }
				href={ courseStatistics + buildQuery({ courseIdInLowerCase, group: id }) }>
				{ name }
			</Link>
		);

		if(i < groupsAsStudent.length - 1) {
			groupsLinks.push(', ');
		}
	}

	return <p className={ styles.root }>Ведомость { groupsLinks }</p>;
}
