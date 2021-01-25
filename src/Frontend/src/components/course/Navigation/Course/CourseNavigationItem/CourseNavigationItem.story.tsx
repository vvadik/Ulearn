import React from "react";
import CourseNavigationItem from "./CourseNavigationItem.js";

const _CourseNavigationItem = (): React.ReactNode => (
	<nav>
		<CourseNavigationItem title="Первое знакомство с C#" progress={ { current: 0, max: 1 } }/>
		<CourseNavigationItem title="Ошибки" progress={ { current: 0, max: 1 } }/>
		<CourseNavigationItem progress={ { current: 0, max: 1 } }
							  title="Тут много текста в одном пункте. Надеюсь, таких длинных текстов у нас не будет, но на всякий случай их нужно учесть"/>
		<CourseNavigationItem progress={ { current: 0, max: 1 } }
							  title="Тутдлинноесловобезпробеловкакжесложноегонабиратьононедолжноничеголоматьвидимолучшийвариантегообрезатьмноготочием"/>
		<CourseNavigationItem title="Текущий модуль" isActive progress={ { current: 0, max: 1 } }/>
		<CourseNavigationItem title="Модуль с прогресс-баром" progress={ { current: 45, max: 100 } }/>
		<CourseNavigationItem
			title="Модуль с заполненным прогресс-баром"
			progress={ { current: 1, max: 1 } }
		/>
		<CourseNavigationItem
			title="Тут много текста в одном пункте. Надеюсь, таких длинных текстов у нас не будет, но на всякий случай их нужно учесть"
			progress={ { current: 45, max: 100 } }
		/>
		<CourseNavigationItem
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
