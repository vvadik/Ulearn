import React from "react";
import NavigationItem from "./NavigationItem";
import { SLIDETYPE } from "../../../../../consts/general";
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
			type={SLIDETYPE.lesson}
			metro={{
				connectToPrev: true,
			}}
		/>
		<NavigationItem
			text="Пункт меню с метро"
			url={""}
			type={SLIDETYPE.lesson}
			metro={{}}
		/>
		<NavigationItem
			text="Пункт меню с иконкой"
			url={""}
			type={SLIDETYPE.quiz}
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
			type={SLIDETYPE.quiz}
			metro={{}}
		/>
		<NavigationItem
			text="Пункт меню с иконкой"
			url={""}
			type={SLIDETYPE.exercise}
			metro={{}}
		/>
		<NavigationItem
			text="Пункт меню с иконкой"
			url={""}
			visited
			type={SLIDETYPE.quiz}
			metro={{
				connectToNext: true,
			}}
		/>
		<NavigationItem
			text="Пункт меню с иконкой"
			url={""}
			visited
			type={SLIDETYPE.exercise}
			metro={{
				connectToPrev: true,
				isLastItem: true,
			}}
		/>
	</nav>
);

_NavigationItem.storyName = "NavigationItem";
