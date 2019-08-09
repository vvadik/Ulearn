import { sortFlashcardsInAuthorsOrder, getNextFlashcardRandomly } from "./flashcardsStirrer";
import { rateTypes } from "../../../../consts/rateTypes";

const { notRated, rate1, rate2, rate3, rate4, rate5 } = rateTypes;

describe('flashcardsStirrer unit sorting should', () => {
	test('sort unseen flashcards in authors order', () => {
		const sequence = [
			new Flashcard(),
			new Flashcard(),
			new Flashcard(),
			new Flashcard(),
			new Flashcard(),
		];

		const result = sortFlashcardsInAuthorsOrder(sequence);

		expect(result).toEqual(sequence);
	});

	test('sort unseen in authors order, then from worst rate to best rate', () => {
		const sequence = [
			new Flashcard(),
			new Flashcard(),
			new Flashcard(rate4),
			new Flashcard(),
			new Flashcard(rate1),
		];
		const answer = [notRated, notRated, notRated, rate1, rate4];

		const result = sortFlashcardsInAuthorsOrder(sequence);

		const resultRates = result.reduce((rates, flashcard) => [...rates, flashcard.rate], []);
		expect(resultRates).toEqual(answer);
	});

	test('sort rated flashcards from worst rate to best rate', () => {
		const sequence = [
			new Flashcard(rate1),
			new Flashcard(rate5),
			new Flashcard(rate3),
			new Flashcard(rate2),
			new Flashcard(rate4),
		];
		const answer = [rate1, rate2, rate3, rate4, rate5];

		const result = sortFlashcardsInAuthorsOrder(sequence);

		const resultRates = result.reduce((rates, flashcard) => [...rates, flashcard.rate], []);
		expect(resultRates).toEqual(answer);
	});

	test('sort rated flashcards with same rate in authors order', () => {
		const sequence = [
			new Flashcard(rate1),
			new Flashcard(rate1),
			new Flashcard(rate1),
			new Flashcard(rate1),
			new Flashcard(rate1),
		];

		const result = sortFlashcardsInAuthorsOrder(sequence);

		expect(result).toEqual(sequence);
	});

	test('sort rated flashcards with same rate in authors order, from worst rate to best rate', () => {
		const sequence = [
			new Flashcard(rate2),
			new Flashcard(rate2),
			new Flashcard(rate1),
			new Flashcard(rate1),
			new Flashcard(rate1),
		];

		const result = sortFlashcardsInAuthorsOrder(sequence);

		expect(result).toEqual([sequence[2], sequence[3], sequence[4], sequence[0], sequence[1]]);
	});
});

describe('flashcardsStirrer course flashcards getter should', () => {
	test('not get same card twice', () => {
		const sequence = [
			new Flashcard(rate2, 1),
			new Flashcard(rate3, 4),
			new Flashcard(rate1, 5),
			new Flashcard(rate5, 2),
			new Flashcard(rate4, 3),
		];

		const first = getNextFlashcardRandomly(sequence, 5);
		sequence.find(fc => fc.id === first.id).lastRateIndex = 6;
		const second = getNextFlashcardRandomly(sequence, 6);

		expect(first).not.toBe(second);
	});

	test('get random flashcard(1 of 100 iterations)', () => {
		const sequence = [
			new Flashcard(rate2, 1),
			new Flashcard(rate3, 4),
			new Flashcard(rate1, 5),
			new Flashcard(rate5, 2),
			new Flashcard(rate4, 3),
		];

		let allSame = true,
			maxTLast = 5,
			previousFlashcard;

		for (let i = 0; i < 100; i++) {
			const flashcard = getNextFlashcardRandomly(sequence, maxTLast);
			if (flashcard !== previousFlashcard) {
				allSame = false;
				break;
			}
			maxTLast++;
			sequence.find(fc => fc.id === flashcard.id).lastRateIndex = maxTLast;
			previousFlashcard = flashcard;
		}

		expect(allSame).toBeFalsy();
	});

	test('built good sequence ( !!!NOT AUTO VALIDATION, NEEDS MANUAL CHECK!!! )', () => {
		const mapNumberToRateType = {
			1: rate1,
			2: rate2,
			3: rate3,
			4: rate4,
			5: rate5,
		};
		const getRandomRate = () => mapNumberToRateType[Math.ceil(Math.random() * 5)];
		const sequence = [
			new Flashcard(getRandomRate(), 1),
			new Flashcard(getRandomRate(), 4),
			new Flashcard(getRandomRate(), 5),
			new Flashcard(getRandomRate(), 2),
			new Flashcard(getRandomRate(), 3),
		];

		console.log(sequence);
		let maxTLast = 5;
		const builtedSequenceHistory = [];

		for (let i = 0; i < 15; i++) {
			const flashcard = getNextFlashcardRandomly(sequence, maxTLast);
			const historyInfo = { ...flashcard };
			const flashcardInSequence = sequence.find(fc => fc.id === flashcard.id);

			maxTLast++;

			flashcardInSequence.lastRateIndex = maxTLast;
			flashcardInSequence.rate = getRandomRate();

			historyInfo.newRate = flashcardInSequence.rate;
			historyInfo.newRateIndex = flashcardInSequence.lastRateIndex;

			builtedSequenceHistory.push(historyInfo);
		}

		console.log(builtedSequenceHistory);
	});
});

let idCounter = 0;

function Flashcard(rate = notRated, lastRateIndex = 0) {
	this.id = idCounter++;
	this.rate = rate;
	this.lastRateIndex = lastRateIndex;
}