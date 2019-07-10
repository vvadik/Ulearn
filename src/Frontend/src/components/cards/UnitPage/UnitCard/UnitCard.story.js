import React from 'react';
import {storiesOf} from '@storybook/react';
import UnitCard from './UnitCard';

storiesOf('Cards/UnitPage/UnitCard', module)
	.add('3 cards with success', () => (
		<UnitCard haveProgress={true} totalFlashcardsCount={3} unitTitle={unitTitle}/>
	))
	.add('3 cards', () => (
		<UnitCard haveProgress={false} totalFlashcardsCount={3} unitTitle={unitTitle}/>
	))
	.add('2 cards with success', () => (
		<UnitCard haveProgress={true} totalFlashcardsCount={2} unitTitle={unitTitle}/>
	))
	.add('2 cards', () => (
		<UnitCard haveProgress={false} totalFlashcardsCount={2} unitTitle={unitTitle}/>
	))
	.add('1 cards with success', () => (
		<UnitCard haveProgress={true} totalFlashcardsCount={1} unitTitle={unitTitle}/>
	))
	.add('1 card', () => (
		<UnitCard haveProgress={false} totalFlashcardsCount={1} unitTitle={unitTitle}/>
	))
	.add('Long title with undefinde totalFlashcardsCount', () => (
		<UnitCard
			unitTitle={getBigTitle()}/>
	))
	.add('Undefined all', () => (
		<UnitCard/>
	));

const unitTitle = "Первое знакомство с C#";

const getBigTitle =
	() => "Большое название, которое все ломает совсем-совсем, " +
		"не люблю такие, да кто любит? - НИКТО... вооот.фыыdfvbg34tgf4fsdaf23rfewf23ыы";
