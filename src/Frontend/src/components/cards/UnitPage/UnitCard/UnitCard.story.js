import React from 'react';
import {storiesOf} from '@storybook/react';
import UnitCard from './UnitCard';

storiesOf('Cards/UnitPage/UnitCard', module)
	.add('3 cards with success', () => (
		<UnitCard haveProgress={true} total={3} unitTitle={unitTitle}/>
	))
	.add('3 cards', () => (
		<UnitCard haveProgress={false} total={3} unitTitle={unitTitle}/>
	))
	.add('2 cards with success', () => (
		<UnitCard haveProgress={true} total={2} unitTitle={unitTitle}/>
	))
	.add('2 cards', () => (
		<UnitCard haveProgress={false} total={2} unitTitle={unitTitle}/>
	))
	.add('1 cards with success', () => (
		<UnitCard haveProgress={true} total={1} unitTitle={unitTitle}/>
	))
	.add('1 card', () => (
		<UnitCard haveProgress={false} total={1} unitTitle={unitTitle}/>
	))
	.add('Long title with undefinde total', () => (
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
