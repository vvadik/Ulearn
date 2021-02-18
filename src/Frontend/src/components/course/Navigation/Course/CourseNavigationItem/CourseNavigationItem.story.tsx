import React from "react";
import CourseNavigationItem from "./CourseNavigationItem";

const mock = () => ({});

const _CourseNavigationItem = (): React.ReactNode => (
	<nav>
		<CourseNavigationItem
			onClick={ mock }
			id={ '1' }
			isActive={ false }
			title="Первое знакомство с C#"
			progress={ { current: 0, max: 1 } }
		/>
		<CourseNavigationItem
			onClick={ mock }
			id={ '1' }
			isActive={ false }
			title="Ошибки"
			progress={ { current: 0, max: 1 } }
		/>
		<CourseNavigationItem
			onClick={ mock }
			id={ '1' }
			isActive={ false }
			progress={ { current: 0, max: 1 } }
			title="Тут много текста в одном пункте. Надеюсь, таких длинных текстов у нас не будет, но на всякий случай их нужно учесть"
		/>
		<CourseNavigationItem
			onClick={ mock }
			id={ '1' }
			isActive={ false }
			progress={ { current: 0, max: 1 } }
			title="Тутдлинноесловобезпробеловкакжесложноегонабиратьононедолжноничеголоматьвидимолучшийвариантегообрезатьмноготочием"
		/>
		<CourseNavigationItem
			onClick={ mock }
			id={ '1' }
			title="Текущий модуль"
			isActive
			progress={ { current: 0, max: 1 } }
		/>
		<CourseNavigationItem
			onClick={ mock }
			id={ '1' }
			isActive={ false }
			title="Модуль с прогресс-баром"
			progress={ { current: 45, max: 100 } }
		/>
		<CourseNavigationItem
			onClick={ mock }
			id={ '1' }
			isActive={ false }
			title="Модуль с заполненным прогресс-баром"
			progress={ { current: 1, max: 1 } }
		/>
		<CourseNavigationItem
			onClick={ mock }
			id={ '1' }
			isActive={ false }
			title="Тут много текста в одном пункте. Надеюсь, таких длинных текстов у нас не будет, но на всякий случай их нужно учесть"
			progress={ { current: 45, max: 100 } }
		/>
		<CourseNavigationItem
			onClick={ mock }
			id={ '1' }
			isActive={ false }
			title="Тутдлинноесловобезпробеловкакжесложноегонабиратьононедолжноничеголоматьвидимолучшийвариантегообрезатьмноготочием"
			progress={ { current: 45, max: 100 } }
		/>
	</nav>
);

export default {
	title: "CourseNavigation",
};

export { _CourseNavigationItem };
_CourseNavigationItem.storyName = 'Items';
