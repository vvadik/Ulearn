import React from "react";
import NextUnit from "./NextUnit.js";


const _NextUnit = (): React.ReactNode =>
	<NextUnit
		unit={ {
			title: 'Следующий модуль',
			slides: [{ slug: '123-213-slug' }],
		} }
	/>;

export default {
	title: "ModuleNavigation",
};
export { _NextUnit };
_NextUnit.storyName = "Следующий модуль";
