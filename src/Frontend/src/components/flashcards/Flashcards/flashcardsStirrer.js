import { rateTypes } from "../../../consts/rateTypes";

export function sortFlashcardsInAuthorsOrder(flashcards) {
	return flashcards.sort((left, right) => {
		const rateLeft = mapRateTypeToNumber[left.rate],
			rateRight = mapRateTypeToNumber[right.rate];

		if (rateLeft === rateRight) {
			return -1;
		}

		return rateRight - rateLeft;
	});
}

const mapRateTypeToNumber = {
	[rateTypes.notRated]: 6,
	[rateTypes.rate1]: 5,
	[rateTypes.rate2]: 4,
	[rateTypes.rate3]: 3,
	[rateTypes.rate4]: 2,
	[rateTypes.rate5]: 1,
};

export function getNextFlashcardRandomly(sequence, maxTLast) {
	const probabilities = sequence
		.map(calculateProbability)
		.sort((a, b) => b.probability - a.probability);

	const probabilitiesSum = probabilities.reduce(
		(sum, { probability }) => sum + probability
		, 0
	);
	const probabilityThreshold = Math.random() * probabilitiesSum;
	let currentProbability = 0;

	for (const { probability, flashcard } of probabilities) {
		currentProbability += probability;

		if (currentProbability > probabilityThreshold) {
			return flashcard;
		}
	}

	function calculateProbability(flashcard) {
		const f = mapRateTypeToNumberInRandom[flashcard.rate];
		const g = maxTLast - flashcard.lastRateIndex;

		const probability = f * (g * g * g);

		return { probability, flashcard };
	}
}

const mapRateTypeToNumberInRandom = {
	[rateTypes.rate1]: 16,
	[rateTypes.rate2]: 8,
	[rateTypes.rate3]: 4,
	[rateTypes.rate4]: 2,
	[rateTypes.rate5]: 1,
};