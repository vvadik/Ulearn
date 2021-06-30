import React from "react";
import Navigation from "./Navigation";
// import { getModuleNavigationProps } from "./stroies.data";

const ModuleNavigation = (): React.ReactNode => (
	<Navigation
		courseItems={ [] }
		courseProgress={ {
			current: 15,
			max: 25,
			inProgress: 5,
		} }
		flashcardsStatistics={ {
			count: 0,
			unratedCount: 0,
		} }
		containsFlashcards={ false }
		courseId={ 'basic' }
		navigationOpened={ true }
		courseTitle="Основы программирования"
		unitTitle="Первое знакомство с C#"
		unitProgress={ {
			current: 50,
			inProgress: 25,
			max: 100,
			statusesBySlides: {},
		} }
		unitItems={ getModuleNavigationProps() }
		nextUnit={ null }
		onCourseClick={ () => ({}) }
	/>
);

export default {
	title: "Navigation",
};
export { ModuleNavigation };
ModuleNavigation.storyName = "Навигация в модуле";
