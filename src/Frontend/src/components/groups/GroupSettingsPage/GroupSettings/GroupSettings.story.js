import React from "react";
import { action } from "@storybook/addon-actions";
import GroupSettings from "./GroupSettings";

import "./groupSettings.less";

export default {
	title: "Settings/GroupSettings",
};

export const Default = () => (
	<GroupSettings
		group={{ test: "test" }}
		updatedFields={{ name: "maria" }}
		onChangeSettings={action("change")}
	/>
);

Default.storyName = "default";
