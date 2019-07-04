import React from 'react';
import {storiesOf} from '@storybook/react';
import UnitPage from './UnitPage';

storiesOf('Cards/UnitPage', module)
	.add('3 cards with success', () => (
		<UnitPage
			guides={guides}
			unitTitle={threeCardsWithSuccess.unitTitle}
			total={threeCardsWithSuccess.total}
			byScore={threeCardsWithSuccess.byScore}
		/>
	))
	.add('3 cards', () => (
		<UnitPage
			guides={guides}
			unitTitle={threeCards.unitTitle}
			total={threeCards.total}
			byScore={threeCards.byScore}
		/>
	))
	.add('2 cards with success', () => (
		<UnitPage
			guides={guides}
			unitTitle={twoCardsWithSuccess.unitTitle}
			total={twoCardsWithSuccess.total}
			byScore={twoCardsWithSuccess.byScore}
		/>
	))
	.add('2 cards', () => (
		<UnitPage
			guides={guides}
			unitTitle={twoCards.unitTitle}
			total={twoCards.total}
			byScore={twoCards.byScore}
		/>
	))
	.add('1 card with success', () => (
		<UnitPage
			guides={guides}
			unitTitle={oneCardWithSuccess.unitTitle}
			total={oneCardWithSuccess.total}
			byScore={oneCardWithSuccess.byScore}
		/>
	))
	.add('1 card', () => (
		<UnitPage
			guides={guides}
			unitTitle={oneCard.unitTitle}
			total={oneCard.total}
			byScore={oneCard.byScore}
		/>
	));

const threeCardsWithSuccess = {
	unitTitle: "Основы программирования 1, знакомство с С#",
	total: 3,
	byScore: {
		unseen: 0,
		1: 0,
		2: 0,
		3: 1,
		4: 1,
		5: 1
	}
};

const threeCards = {
	...threeCardsWithSuccess,
	byScore: {
		unseen: 3,
		1: 0,
		2: 0,
		3: 0,
		4: 0,
		5: 0
	}
};

const twoCardsWithSuccess = {
	unitTitle: "Основы программирования 2",
	total: 2,
	byScore: {
		unseen: 0,
		1: 0,
		2: 1,
		3: 1,
		4: 0,
		5: 0
	}
};

const twoCards = {
	...twoCardsWithSuccess,
	byScore: {
		unseen: 2,
		1: 0,
		2: 0,
		3: 0,
		4: 0,
		5: 0
	}
};

const oneCardWithSuccess = {
	unitTitle: "Основы программирования 3, знакомство с JS, принципы agile",
	total: 1,
	byScore: {
		unseen: 0,
		1: 1,
		2: 0,
		3: 0,
		4: 0,
		5: 0
	}
};

const oneCard = {
	...oneCardWithSuccess,
	byScore: {
		unseen: 1,
		1: 0,
		2: 0,
		3: 0,
		4: 0,
		5: 0
	}
};

const guides = [
	"Подумайте над вопросом, перед тем как смотреть ответ.",
	"Оцените, на сколько хорошо вы знали ответ. Карточки, которые вы знаете плохо, будут показываться чаще",
	"Регулярно пересматривайте карточки, даже если вы уверенны в своих знаниях. Чем чаще использовать карточки, тем лучше они запоминаются.",
	"делай так", 'не делай так'
];
