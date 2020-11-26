import React from "react";
import NavigationItem from "./NavigationItem";
import { SlideType } from "src/models/slide";
import StoryRouter from "storybook-react-router";

export default {
	title: "ModuleNavigation",
	decorators: [StoryRouter()],
};

export const _NavigationItem = () => (
	<nav>
		<NavigationItem text="Пункт меню со счетом" score={0.45} url={""} />
		<NavigationItem
			text="Пункт меню со счетом и описанием"
			url={""}
			description="Задание"
			score={0}
		/>
		<NavigationItem
			text="Пункт меню с метро"
			url={""}
			visited
			type={SlideType.Lesson}
			metro={{
				connectToPrev: true,
			}}
		/>
		<NavigationItem
			text="Пункт меню с метро"
			url={""}
			type={SlideType.Lesson}
			metro={{}}
		/>
		<NavigationItem
			text="Пункт меню с иконкой"
			url={""}
			type={SlideType.Quiz}
			metro={{}}
		/>
		<NavigationItem
			text="Пункт меню с иконкой и описанием и счетом"
			url={""}
			hasMetro
			score={3}
			maxScore={5}
			description="Ждет код-ревью • 3 попытки осталось"
			isActive
			type={SlideType.Quiz}
			metro={{}}
		/>
		<NavigationItem
			text="Пункт меню с иконкой"
			url={""}
			type={SlideType.Exercise}
			metro={{}}
		/>
		<NavigationItem
			text="Пункт меню с иконкой"
			url={""}
			visited
			type={SlideType.Quiz}
			metro={{
				connectToNext: true,
			}}
		/>
		<NavigationItem
			text="Пункт меню с иконкой"
			url={""}
			visited
			type={SlideType.Exercise}
			metro={{
				connectToPrev: true,
				isLastItem: true,
			}}
		/>
	</nav>
);

_NavigationItem.storyName = "NavigationItem";
