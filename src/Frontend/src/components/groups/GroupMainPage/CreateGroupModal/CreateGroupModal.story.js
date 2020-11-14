import React from "react";
import { action } from "@storybook/addon-actions";
import CreateGroupModal from "./CreateGroupModal";

import "./createGroupModal.less";

export default {
	title: "Group/CreateGroupModal",
};

export const Default = () => (
	<CreateGroupModal onCloseModal={action("onCloseModal")} courseId={"123"} />
);

Default.storyName = "default";
