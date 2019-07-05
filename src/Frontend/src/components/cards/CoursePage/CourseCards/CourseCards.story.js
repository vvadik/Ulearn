import CourseCards from './CourseCards';
import React from 'react';
import {storiesOf} from '@storybook/react';

storiesOf('Cards/CoursePage/CourseCards', module)
	.add('def', () => (
		<CourseCards cardsByUnits={cardsByUnits}/>
	));

let unitIdCounter = 0;

const unitCard = (title, unlocked = true) => {
	unitIdCounter++;
	return {
		unitTitle: title,
		unlocked: unlocked,
		cardsCount: 13,
		unitId: `${unitIdCounter}`
	};
};

const cardsByUnits = [
	unitCard('Первое знакомство с C#'),
	unitCard('Ошибка'),
	unitCard('Циклы'),
	unitCard('Массивы'),
	unitCard('Коллекции, строки, файлы'),
	unitCard('Классы'),
	unitCard('Сложность алгоритмов', false),
	unitCard('Сложность сложности', false),
	unitCard('Секретный раздел', false),
	unitCard('Секретный раздел с длинным названием, попытка вынести количество карточек за пределы контейнера :)', false)
];
