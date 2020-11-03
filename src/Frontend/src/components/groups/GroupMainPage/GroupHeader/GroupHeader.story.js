import React from "react";
import { action } from "@storybook/addon-actions";
import GroupHeader from "./GroupHeader";

import "./groupHeader.less";

export default {
	title: "Group/GroupHeader",
};

export const Default = () => (
	<GroupHeader onTabChange={action("click")} filter="hello" />
);

Default.storyName = "default";
