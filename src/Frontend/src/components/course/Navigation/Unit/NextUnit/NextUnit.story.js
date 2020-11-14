import React from "react";
import NextUnit from "./NextUnit";
import StoryRouter from "storybook-react-router";

export default {
	title: "ModuleNavigation",
	decorators: [StoryRouter()],
};

export const СледующийМодуль = () => <NextUnit />;

СледующийМодуль.storyName = "Следующий модуль";
