import React from 'react';
import {storiesOf} from '@storybook/react';
import CourseFlashcardsPage from "./CourseFlashcardsPage";
import cardsByUnitExample from "./CourseCards/cardsByUnitExample";

storiesOf('Cards/CoursePage', module)
	.add('default', () => (
		<CourseFlashcardsPage
			guides={guides}
			flashcardsInfos={cardsByUnitExample}
		/>
	));

const guides = [
	"Подумайте над вопросом, перед тем как смотреть ответ.",
	"Оцените, на сколько хорошо вы знали ответ. Карточки, которые вы знаете плохо, будут показываться чаще",
	"Регулярно пересматривайте карточки, даже если вы уверенны в своих знаниях. Чем чаще использовать карточки, тем лучше они запоминаются.",
	"делай так", 'не делай так'
];
