import React from "react";
import CourseNavigationContent from "./CourseNavigationContent";

export default {
	title: "CourseNavigation",
};

export const _CourseNavigationContent = () => (
	<CourseNavigationContent items={getCourseNav()} />
);

_CourseNavigationContent.storyName = "CourseNavigationContent";

function getCourseNav() {
	return [
		{
			title: "Первое знакомство с C#",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N0",
			progress: 0.84,
		},
		{
			title: "Ошибки",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N1",
			progress: 1,
		},
		{
			title: "Ветвления",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N2",
			progress: 1,
		},
		{
			title: "Циклы",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N3",
			progress: 0.45,
		},
		{
			title: "Массивы",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N4",
		},
		{
			title: "Коллекции, строки, файлы",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N5",
		},
		{
			title: "Тестирование",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N6",
		},
		{
			title: "Сложность алгоритмов",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N7",
		},
		{
			title: "Рекурсивные алгоритмы",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N8",
		},
		{
			title: "Поиск и сортировка",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N9",
		},
		{
			title: "Практикум",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N10",
		},
		{
			title: "Основы ООП",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N11",
		},
		{
			title: "Наследование",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N12",
		},
		{
			title: "Целостность данных",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N13",
		},
		{
			title: "Структуры",
			url:
				"https://ulearn.me/Course/BasicProgramming/Ob_yavlenie_struktury_2f0b0caa-ce22-4068-93bb-e5c1a0f8a2a4#N14",
		},
	];
}
