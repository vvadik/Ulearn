import React from 'react';
import {storiesOf} from '@storybook/react';
import CoursePage from "./CoursePage";
import cardsByUnitExample from "./CourseCards/cardsByUnitExample";

storiesOf('Cards/CoursePage', module)
	.add('default', () => (
		<CoursePage
			guides={guides}
			cardsByUnits={cardsByUnitExample}
		/>
	));

const guides = [
	"Подумайте над вопросом, перед тем как смотреть ответ.",
	"Оцените, на сколько хорошо вы знали ответ. Карточки, которые вы знаете плохо, будут показываться чаще",
	"Регулярно пересматривайте карточки, даже если вы уверенны в своих знаниях. Чем чаще использовать карточки, тем лучше они запоминаются.",
	"делай так", 'не делай так'
];
