import { rateTypes } from "../../../../consts/rateTypes";

const { notRated, } = rateTypes;

let idCounter = 0;

export default function Flashcard(rate = notRated, lastRateIndex = 0) {
	this.id = idCounter++;
	this.rate = rate;
	this.lastRateIndex = lastRateIndex;
}
