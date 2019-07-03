import React from 'react';
import {storiesOf} from '@storybook/react';
import UnitCard from './UnitCard';

const unitCardsData = {
	title: "Первое знакомство с C#",
	cards13: [
		'1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13'
	],
	cards1: [
		'1'
	],
	cards2: [
		'1', '2'
	],
};

storiesOf('Cards/UnitPage/UnitCard', module)
	.add('13 cards with success', () => (
		<UnitCard title={unitCardsData.title} cards={unitCardsData.cards13} isCompleted={true}/>
	))
	.add('1 card', () => (
		<UnitCard title={unitCardsData.title} cards={unitCardsData.cards1}/>
	))
	.add('2 cards', () => (
		<UnitCard title={unitCardsData.title} cards={unitCardsData.cards2}/>
	))
	.add('Long title', () => (
		<UnitCard title={"Большое название, которое все ломает совсем-совсем, не люблю такие, да кто любит? - НИКТО... вооот.фыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыы"} cards={unitCardsData.cards2}/>
	));