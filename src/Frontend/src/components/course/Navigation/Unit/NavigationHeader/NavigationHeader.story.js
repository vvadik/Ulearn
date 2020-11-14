import React from "react";
import NavigationHeader from "./NavigationHeader";

export default {
	title: "ModuleNavigation",
};

export const ШапкаВНавигации = () => (
	<div>
		<NavigationHeader
			title="Первое знакомство с C#"
			courseName="Основы программирования"
			courseUrl="/BasicProgramming"
		/>
	</div>
);

ШапкаВНавигации.storyName = "Шапка в навигации";
