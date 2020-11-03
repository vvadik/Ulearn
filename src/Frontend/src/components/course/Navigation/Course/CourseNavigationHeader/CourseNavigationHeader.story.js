import React from "react";
import CourseNavigationHeader from "./CourseNavigationHeader";

export default {
	title: "CourseNavigation",
};

export const CourseNavigationHeaderШапкаВНавигацииКурса = () => (
	<div>
		<CourseNavigationHeader
			title="Основы программирования"
			description={getDescription()}
			progress={0}
		/>
		<CourseNavigationHeader
			title="Основы программирования"
			description={getDescription()}
			progress={0.56}
		/>
		<CourseNavigationHeader title="Основы программирования" progress={0} />
		<CourseNavigationHeader title="Основы программирования" progress={1} />
	</div>
);

CourseNavigationHeaderШапкаВНавигацииКурса.storyName = "CourseNavigationHeader: Шапка в навигации курса";

function getDescription() {
	return `Знакомство с основами синтаксиса C#, 
    стандартными классами .NET, 
    с основами ООП и базовыми алгоритмами.`;
}
