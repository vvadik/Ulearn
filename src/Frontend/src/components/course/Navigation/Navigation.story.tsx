import React from "react";
import Navigation from "./Navigation";
import { getModuleNavigationProps } from "./stroies.data";

const ModuleNavigation = (): React.ReactNode => (
	<Navigation
		courseItems={ [] }
		courseProgress={ {
			current: 15,
			max: 25,
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
			doneSlidesCount: 50,
			inProgressSlidesCount: 25,
			slidesCount: 100,
			statusesBySlides: {},
			current: 10,
			max: 20,
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
