import React from "react";
import { storiesOf } from "@storybook/react";
import NavigationItem from './NavigationItem';
import { itemTypes } from '../../constants';
import StoryRouter from 'storybook-react-router';


storiesOf("ModuleNavigation", module)
	.addDecorator(StoryRouter())
	.add("NavigationItem", () => (
		<nav>
			<NavigationItem text='Пункт меню со счетом' score={ 0.45 } url={''} />
			<NavigationItem text='Пункт меню со счетом и описанием' url={''} description='Задание' score={ 0 } />
			<NavigationItem text='Пункт меню с метро'
							url={''}
							visited
							type={ itemTypes.lesson }
							metro={{
								connectToPrev: true,
			}} />
			<NavigationItem text='Пункт меню с метро'
							url={''}
							type={ itemTypes.lesson }
							metro={{}}
			/>
			<NavigationItem text='Пункт меню с иконкой'
							url={''}
							type={ itemTypes.quiz }
							metro={{}} />
			<NavigationItem text='Пункт меню с иконкой и описанием и счетом'
							url={''}
							hasMetro
							score={ 3 }
							maxScore={ 5 }
							description='Ждет код-ревью • 3 попытки осталось'
							isActive
							type={ itemTypes.quiz }
							metro={{}} />
			<NavigationItem text='Пункт меню с иконкой'
							url={''}
							type={ itemTypes.exercise }
							metro={{}} />
			<NavigationItem text='Пункт меню с иконкой'
							url={''}
							visited
							type={ itemTypes.quiz }
							metro={{
								connectToNext: true,
			}} />
			<NavigationItem text='Пункт меню с иконкой'
							url={''}
							visited
							type={ itemTypes.exercise }
							metro={{
				connectToPrev: true,
				isLastItem: true,
			}} />

		</nav>
	));

