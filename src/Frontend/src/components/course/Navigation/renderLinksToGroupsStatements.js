import React from "react"

import Link from "@skbkontur/react-ui/Link";

import { courseStatistics } from "../../../consts/routes";
import { buildQuery } from "../../../utils";

export default function renderLinkToGroupStatement(groupsAsStudent) {
	if (groupsAsStudent.length === 0) {
		return null;
	}

	const groupsLinks = ['Ведомость '];

	for (let i = 0; i < groupsAsStudent.length; i++) {
		const { id, courseId, name, } = groupsAsStudent[i];

		groupsLinks.push(
			<Link
				key={ id }
				href={ courseStatistics + buildQuery({ courseId, unitId: id }) }>
				{ name }
			</Link>
		);

		if (i < groupsAsStudent.length - 1) {
			groupsLinks.push(', ');
		}
	}

	return groupsLinks;
}