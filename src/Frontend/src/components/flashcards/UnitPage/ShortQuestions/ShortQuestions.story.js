import React from 'react';
import {storiesOf} from '@storybook/react';
import ShortQuestions from "./ShortQuestions";
import shortQuestionsExample from "./shortQuestionsExample";

storiesOf('Cards/UnitPage/ShortQuestions', module)
	.add('default', () => (
		<ShortQuestions
			questionsWithAnswers={shortQuestionsExample}
		/>
	));
