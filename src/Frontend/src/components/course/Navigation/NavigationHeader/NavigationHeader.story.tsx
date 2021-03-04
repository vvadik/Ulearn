import React from "react";
import NavigationHeader from "./NavigationHeader.js";

const _CourseNavigationHeader = (): React.ReactNode => (
	<div>
		<NavigationHeader
			title="Основы программирования"
			description={ getDescription() }
			courseProgress={ { current: 0, max: 0 } }
			groupsAsStudent={ [] }
		/>
		<NavigationHeader
			title="Основы программирования"
			description={ getDescription() }
			courseProgress={ { current: 56, max: 100 } }
			groupsAsStudent={ [] }
		/>
		<NavigationHeader
			title="Основы программирования"
			courseProgress={ { current: 0, max: 0 } }
			groupsAsStudent={ [] }
		/>
		<NavigationHeader
			title="Основы программирования"
			courseProgress={ { current: 1, max: 1 } }
			groupsAsStudent={ [] }
		/>
	</div>
);

function getDescription() {
	return `Знакомство с основами синтаксиса C#, 
    стандартными классами .NET, 
    с основами ООП и базовыми алгоритмами.`;
}

export default {
	title: "CourseNavigation",
};

export { _CourseNavigationHeader };
_CourseNavigationHeader.storyName = 'Headers';
