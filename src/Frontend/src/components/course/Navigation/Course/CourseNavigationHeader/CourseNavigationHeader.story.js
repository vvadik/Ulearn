import React from "react";
import { storiesOf } from "@storybook/react";
import CourseNavigationHeader from './CourseNavigationHeader';


storiesOf("CourseNavigation", module)
	.add("CourseNavigationHeader: Шапка в навигации курса", () => (
		<div>
			<CourseNavigationHeader title='Основы программирования'
									description={getDescription()}
									progress={ 0 }
			/>
			<CourseNavigationHeader title='Основы программирования'
									description={getDescription()}
									progress={ 0.56 }
			/>
			<CourseNavigationHeader title='Основы программирования'
									progress={ 0 }
			/>
			<CourseNavigationHeader title='Основы программирования'
									progress={ 1 }
			/>
		</div>
	));

function getDescription () {
	return `Знакомство с основами синтаксиса C#, 
	стандартными классами .NET, 
	с основами ООП и базовыми алгоритмами.`;
}
