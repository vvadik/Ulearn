import React from "react";
import GroupHeader from "./GroupHeader.js";

import "./groupHeader.less";

export default {
	title: "Group/GroupHeader",
};

export const Default = (): React.ReactNode => (
	<GroupHeader onTabChange={ () => ({}) } filter="hello"/>
);

Default.storyName = "default";
