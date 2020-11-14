import React from "react";
import CourseNavigationItem from "./CourseNavigationItem";

export default {
	title: "CourseNavigation",
};

export const _CourseNavigationItem = () => (
	<nav>
		<CourseNavigationItem text="Первое знакомство с C#" />
		<CourseNavigationItem text="Ошибки" />
		<CourseNavigationItem text="Тут много текста в одном пункте. Надеюсь, таких длинных текстов у нас не будет, но на всякий случай их нужно учесть" />
		<CourseNavigationItem text="Тутдлинноесловобезпробеловкакжесложноегонабиратьононедолжноничеголоматьвидимолучшийвариантегообрезатьмноготочием" />
		<CourseNavigationItem text="Текущий модуль" isActive />
		<CourseNavigationItem text="Модуль с прогресс-баром" progress={0.45} />
		<CourseNavigationItem
			text="Модуль с заполненным прогресс-баром"
			progress={1}
		/>
		<CourseNavigationItem
			text="Тут много текста в одном пункте. Надеюсь, таких длинных текстов у нас не будет, но на всякий случай их нужно учесть"
			progress={0.45}
		/>
		<CourseNavigationItem
			text="Тутдлинноесловобезпробеловкакжесложноегонабиратьононедолжноничеголоматьвидимолучшийвариантегообрезатьмноготочием"
			progress={0.45}
		/>
	</nav>
);

_CourseNavigationItem.storyName = "CourseNavigationItem";
