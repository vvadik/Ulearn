import React from 'react';
import {storiesOf} from '@storybook/react';
import ShortQuestions from "./ShortQuestions";

storiesOf('Cards/UnitPage/ShortQuestions', module)
	.add('default', () => (
		<ShortQuestions
			questions={['Почему 1 больше 2?', 'Это так?', getLongQuestion()]}
			answers={['Потому что 2 меньше 1... наверное', 'Нет, не так', getLongAnswer()]}/>
	));

const getLongQuestion =
	() => 'Если я спрошу очень большой вопрос, то сломается ли верстка? Если да, то почему? Как это поправить? Или не стоит? как думаешь? Почему?';

const getLongAnswer =
	() => 'Большой ответ также может сломать все, что захочет, должно быть стоит лучше приглядываться к стилям. И так же надо написать ещё 5 слов чтобы было побольше слов.';
