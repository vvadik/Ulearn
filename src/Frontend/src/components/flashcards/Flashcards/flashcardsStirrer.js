import { rateTypes } from "../../../consts/rateTypes";

export default function mixFlashcards(sequence, maxTLast) {
	return sequence.sort((first, second) => {
		return calculateProbability(second) - calculateProbability(first);
	});

	function calculateProbability(flashcard) {
		const f = mapRateTypeToNumber[flashcard.rate];
		let g = maxTLast - flashcard.lastRateIndex - 1;

		return f * g;
	}
}

const mapRateTypeToNumber = {
	[rateTypes.notRated]: 100000,
	[rateTypes.rate1]: 10,
	[rateTypes.rate2]: 8,
	[rateTypes.rate3]: 6,
	[rateTypes.rate4]: 4,
	[rateTypes.rate5]: 1,
};