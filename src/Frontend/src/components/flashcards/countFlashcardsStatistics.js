import { RateTypes } from "../../consts/rateTypes";

export default function countFlashcardsStatistics(flashcards) {
	const statistics = Object.values(RateTypes)
		.reduce((stats, rateType) => {
			return { ...stats, [rateType]: 0 }
		}, {});

	for (const flashcard of flashcards) {
		statistics[flashcard.rate]++;
	}

	return statistics;
}
