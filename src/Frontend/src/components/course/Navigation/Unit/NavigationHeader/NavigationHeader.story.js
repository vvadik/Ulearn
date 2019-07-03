import React from "react";
import { storiesOf } from "@storybook/react";
import NavigationHeader from './NavigationHeader';


storiesOf("ModuleNavigation", module)
	.add("Шапка в навигации", () => (
		<div>
			<NavigationHeader title='Первое знакомство с C#'
							  courseName='Основы программирования'
							  courseUrl='/BasicProgramming'
			/>
		</div>
	));

