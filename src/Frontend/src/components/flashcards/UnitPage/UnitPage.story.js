import React from "react";
import UnitPage from "./UnitPage";
import shortQuestionsExample from "./ShortQuestions/shortQuestionsExample";

export default {
	title: "Cards/UnitPage",
};

export const _3CardsWithSuccess = () => (
	<UnitPage
		guides={guides}
		unitTitle={threeCardsWithSuccess.unitTitle}
		totalFlashcardsCount={threeCardsWithSuccess.totalFlashcardsCount}
		statistics={threeCardsWithSuccess.statistics}
		questionsWithAnswers={shortQuestionsExample}
	/>
);

_3CardsWithSuccess.storyName = "3 cards with success";

export const _3Cards = () => (
	<UnitPage
		guides={guides}
		unitTitle={threeCards.unitTitle}
		totalFlashcardsCount={threeCards.totalFlashcardsCount}
		statistics={threeCards.statistics}
		questionsWithAnswers={shortQuestionsExample}
	/>
);

_3Cards.storyName = "3 cards";

export const _2CardsWithSuccess = () => (
	<UnitPage
		guides={guides}
		unitTitle={twoCardsWithSuccess.unitTitle}
		totalFlashcardsCount={twoCardsWithSuccess.totalFlashcardsCount}
		statistics={twoCardsWithSuccess.statistics}
		questionsWithAnswers={shortQuestionsExample}
	/>
);

_2CardsWithSuccess.storyName = "2 cards with success";

export const _2Cards = () => (
	<UnitPage
		guides={guides}
		unitTitle={twoCards.unitTitle}
		totalFlashcardsCount={twoCards.totalFlashcardsCount}
		statistics={twoCards.statistics}
		questionsWithAnswers={shortQuestionsExample}
	/>
);

_2Cards.storyName = "2 cards";

export const _1CardWithSuccess = () => (
	<UnitPage
		guides={guides}
		unitTitle={oneCardWithSuccess.unitTitle}
		totalFlashcardsCount={oneCardWithSuccess.totalFlashcardsCount}
		statistics={oneCardWithSuccess.statistics}
		questionsWithAnswers={shortQuestionsExample}
	/>
);

_1CardWithSuccess.storyName = "1 card with success";

export const _1Card = () => (
	<UnitPage
		guides={guides}
		unitTitle={oneCard.unitTitle}
		totalFlashcardsCount={oneCard.totalFlashcardsCount}
		statistics={oneCard.statistics}
		questionsWithAnswers={shortQuestionsExample}
	/>
);

_1Card.storyName = "1 card";

function createCard(
	unitTitle,
	{ notRated = 0, rate1 = 0, rate2 = 0, rate3 = 0, rate4 = 0, rate5 = 0 }
) {
	return {
		unitTitle: unitTitle,
		totalFlashcardsCount: notRated + rate1 + rate2 + rate3 + rate4 + rate5,
		statistics: {
			notRated: notRated,
			rate1: rate1,
			rate2: rate2,
			rate3: rate3,
			rate4: rate4,
			rate5: rate5,
		},
	};
}

const threeCardsWithSuccess = createCard(
	"Основы программирования 1, знакомство с С#",
	{
		rate3: 1,
		rate4: 1,
		rate5: 1,
	}
);

const threeCards = createCard("Основы программирования 1, знакомство с С#", {
	notRated: 3,
});

const twoCardsWithSuccess = createCard("Основы программирования 2", {
	rate2: 1,
	rate3: 1,
});

const twoCards = createCard("Основы программирования 2", {
	notRated: 2,
});

const oneCardWithSuccess = createCard(
	"Основы программирования 3, знакомство с JS, принципы agile",
	{
		rate1: 1,
	}
);

const oneCard = createCard(
	"Основы программирования 3, знакомство с JS, принципы agile",
	{
		notRated: 1,
	}
);

const guides = [
	"Подумайте над вопросом, перед тем как смотреть ответ.",
	"Оцените, на сколько хорошо вы знали ответ. Карточки, которые вы знаете плохо, будут показываться чаще",
	"Регулярно пересматривайте карточки, даже если вы уверенны в своих знаниях. Чем чаще использовать карточки, тем лучше они запоминаются.",
	"делай так",
	"не делай так",
];
