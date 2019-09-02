import { rateTypes } from "../../../../consts/rateTypes";

export function sortFlashcardsInAuthorsOrderWithRate(flashcards) {
	const copy = [...flashcards];
	return copy.sort((left, right) => {
		return mapRateTypeToSortingCoefficient[right.rate] - mapRateTypeToSortingCoefficient[left.rate];
	});
}

const mapRateTypeToSortingCoefficient = {
	[rateTypes.notRated]: 6,
	[rateTypes.rate1]: 5,
	[rateTypes.rate2]: 4,
	[rateTypes.rate3]: 3,
	[rateTypes.rate4]: 2,
	[rateTypes.rate5]: 1,
};

export function getNextFlashcardRandomly(sequence, maxLastRateIndex) {
	const probabilities = sequence
		.map(calculateProbability)
		.sort((a, b) => b.probability - a.probability);

	const probabilitiesSum = probabilities.reduce(
		(sum, { probability }) => sum + probability, 0);

	//A randomizer is used to select a card.
	const probabilityThreshold = Math.random() * probabilitiesSum;
	let currentProbability = 0;

	for (const { probability, flashcard } of probabilities) {
		currentProbability += probability;

		if (currentProbability > probabilityThreshold) {
			return flashcard;
		}
	}

	return '';

	function calculateProbability(flashcard) {
		const ratingCoefficient = mapRateTypeToProbabilityCoefficient[flashcard.rate];
		const timeCoefficient = maxLastRateIndex - flashcard.lastRateIndex;

		// For each card, the probability with which it will be shown next is considered.
		// This probability depends on:
		// Card rating;
		// The time that the card did not show ( priority );

		const probability = ratingCoefficient * timeCoefficient * timeCoefficient * timeCoefficient;

		return { probability, flashcard };
	}
}

const mapRateTypeToProbabilityCoefficient = {
	[rateTypes.rate1]: 16,
	[rateTypes.rate2]: 8,
	[rateTypes.rate3]: 4,
	[rateTypes.rate4]: 2,
	[rateTypes.rate5]: 1,
};