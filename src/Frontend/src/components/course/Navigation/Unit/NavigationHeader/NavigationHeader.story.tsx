import React from "react";
import NavigationHeader from "./NavigationHeader.js";


const Header = (): React.ReactNode => (
	<div>
		<NavigationHeader
			createRef={ () => ({}) }
			groupsAsStudent={ [] }
			progress={ { current: 5, max: 10 } }
			title="Первое знакомство с C#"
			courseName="Основы программирования"
			courseUrl="/BasicProgramming"
		/>
	</div>
);

export default {
	title: "ModuleNavigation",
};
export { Header };
Header.storyName = "Шапка в навигации";
