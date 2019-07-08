import React from 'react';
import {storiesOf} from '@storybook/react';
import ProgressBar from "./ProgressBar";

storiesOf('Cards/ProgressBar', module)
	.add('scores: 1,1,2,5,10,18', () => (
		<ProgressBar
			byScore={{
				unseen: 1,
				1: 1,
				2: 2,
				3: 5,
				4: 10,
				5: 18
			}}
			total={37}/>
	))
	.add('scores: 10,0,0,0,0,0', () => (
		<ProgressBar
			byScore={{
				unseen: 10,
				1: 0,
				2: 0,
				3: 0,
				4: 0,
				5: 0
			}}
			total={10}/>
	))
	.add('scores: 2,2,2,2,2,2', () => (
		<ProgressBar
			byScore={{
				unseen: 2,
				1: 2,
				2: 2,
				3: 2,
				4: 2,
				5: 2
			}}
			total={12}/>
	));
