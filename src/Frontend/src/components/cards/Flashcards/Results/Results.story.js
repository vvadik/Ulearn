import React from 'react';
import {storiesOf} from '@storybook/react';
import Results from './Results';

storiesOf('Cards/Results', module)
	.add('default', () => (
		<Results handleClick={() => {
		}}/>
	));
