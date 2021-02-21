import React from "react";
import GroupInfo from "./GroupInfo.js";

import "./groupInfo.less";

export default {
	title: "Group/GroupInfo",
};

export const Default = (): React.ReactNode => <GroupInfo courseId={ '1' } group={ getGroup() }/>;

Default.storyName = "default";

function getGroup() {
	return {
		id: 17,
		name: "asdfasdfasdfasdf",
		isArchived: false,
		owner: {
			id: "4052ea63-34dd-4398-b8bb-ac4e6a85d1d0",
			visibleName: "paradeeva",
			avatarUrl: null,
		},
		inviteHash: "b7638c37-62c6-49a9-898c-38788169987c",
		isInviteLinkEnabled: true,
		isManualCheckingEnabled: false,
		isManualCheckingEnabledForOldSolutions: false,
		defaultProhibitFurtherReview: true,
		canStudentsSeeGroupProgress: true,
		studentsCount: 0,
		accesses: [],
		apiUrl: "/groups/17",
	};
}
