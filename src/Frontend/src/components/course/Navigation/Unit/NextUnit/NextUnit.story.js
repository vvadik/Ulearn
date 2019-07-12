import React from 'react';
import { storiesOf } from '@storybook/react';
import NextUnit from './NextUnit';
import StoryRouter from 'storybook-react-router';



storiesOf('ModuleNavigation', module)
	.addDecorator(StoryRouter())
	.add('Следующий модуль', () => (
		<NextUnit />
	));

