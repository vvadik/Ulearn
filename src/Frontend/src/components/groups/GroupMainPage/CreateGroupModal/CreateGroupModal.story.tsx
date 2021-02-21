import React from "react";
import CreateGroupModal from "./CreateGroupModal.js";

import "./createGroupModal.less";

export default {
	title: "Group/CreateGroupModal",
};

export const Default = (): React.ReactNode => (
	<CreateGroupModal onCloseModal={ () => ({}) } courseId={ "123" }/>
);

Default.storyName = "default";
