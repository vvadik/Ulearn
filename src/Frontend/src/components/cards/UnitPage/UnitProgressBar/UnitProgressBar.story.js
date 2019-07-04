import React from 'react';
import {storiesOf} from '@storybook/react';
import UnitProgressBar from "./UnitProgressBar";

storiesOf('Cards/UnitPage/UnitProgressBar', module)
	.add('scores: 1,1,2,5,10,18', () => (
		<UnitProgressBar byScore={[1, 1, 2, 5, 10, 18]}/>
	))
	.add('scores: 10,0,0,0,0,0', () => (
		<UnitProgressBar byScore={[10, 0, 0, 0, 0, 0]}/>
	))
	.add('scores: 2,2,2,2,2,2', () => (
		<UnitProgressBar byScore={[2, 2, 2, 2, 2, 2]}/>
	));