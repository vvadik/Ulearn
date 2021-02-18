import React from "react";
import NavigationHeader from "./NavigationHeader";


const Header = (): React.ReactNode => (
	<div>
		<NavigationHeader
			onClick={ () => ({}) }
			createRef={ React.createRef() }
			groupsAsStudent={ [] }
			progress={ { current: 5, max: 10 } }
			title="Первое знакомство с C#"
			courseName="Основы программирования"
		/>
	</div>
);

export default {
	title: "ModuleNavigation",
};
export { Header };
Header.storyName = "Шапка в навигации";
