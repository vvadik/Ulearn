import React from "react";
import NavigationItem from "./NavigationItem.js";
import { SlideType } from "src/models/slide";

const _NavigationItem = (): React.ReactNode => (
	<nav>
		<NavigationItem text="Пункт меню со счетом" score={ 0.45 } url={ "" }/>
		<NavigationItem
			text="Пункт меню со счетом и описанием"
			url={ "" }
			description="Задание"
			score={ 0 }
		/>
		<NavigationItem
			text="Пункт меню с метро"
			url={ "" }
			visited
			type={ SlideType.Lesson }
			metro={ {
				connectToPrev: true,
			} }
		/>
		<NavigationItem
			text="Пункт меню с метро"
			url={ "" }
			type={ SlideType.Lesson }
			metro={ {} }
		/>
		<NavigationItem
			text="Пункт меню с иконкой"
			url={ "" }
			type={ SlideType.Quiz }
			metro={ {} }
		/>
		<NavigationItem
			text="Пункт меню с иконкой и описанием и счетом"
			url={ "" }
			hasMetro
			score={ 3 }
			maxScore={ 5 }
			description="Ждет код-ревью • 3 попытки осталось"
			isActive
			type={ SlideType.Quiz }
			metro={ {} }
		/>
		<NavigationItem
			text="Пункт меню с иконкой"
			url={ "" }
			type={ SlideType.Exercise }
			metro={ {} }
		/>
		<NavigationItem
			text="Пункт меню с иконкой"
			url={ "" }
			visited
			type={ SlideType.Quiz }
			metro={ {
				connectToNext: true,
			} }
		/>
		<NavigationItem
			text="Пункт меню с иконкой"
			url={ "" }
			visited
			type={ SlideType.Exercise }
			metro={ {
				connectToPrev: true,
				isLastItem: true,
			} }
		/>
	</nav>
);

export default {
	title: "ModuleNavigation",
};
export { _NavigationItem };
_NavigationItem.storyName = "navigation items";
