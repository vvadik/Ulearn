import React from "react";
import { storiesOf } from "@storybook/react";
import NavigationItem from './NavigationItem';
import { itemTypes } from '../../constants';




storiesOf("ModuleNavigation", module)
	.add("NavigationItem", () => (
		<nav>
			<NavigationItem text='Пункт меню со счетом' score={ 0.45 } />
			<NavigationItem text='Пункт меню со счетом и описанием' description='Задание' score={ 0 } />
			<NavigationItem text='Пункт меню с метро' metro={{
				complete: true,
				type: itemTypes.lesson,
				connectToPrev: true,
			}} />
			<NavigationItem text='Пункт меню с метро' metro={{complete: false, type: itemTypes.lesson}} />
			<NavigationItem text='Пункт меню с иконкой' metro={{complete: false, type: itemTypes.quiz}} />
			<NavigationItem text='Пункт меню с иконкой и описанием и счетом'
							hasMetro
							score={ 0.68443 }
							description='Ждет код-ревью • 3 попытки осталось'
							isActive
							metro={{complete: false, type: itemTypes.quiz}} />
			<NavigationItem text='Пункт меню с иконкой' metro={{complete: false, type: itemTypes.exercise}} />
			<NavigationItem text='Пункт меню с иконкой' metro={{
				complete: true,
				type: itemTypes.quiz,
				connectToNext: true,
			}} />
			<NavigationItem text='Пункт меню с иконкой' metro={{
				complete: true,
				type: itemTypes.exercise,
				connectToPrev: true,
				isLastItem: true,
			}} />

		</nav>
	));

