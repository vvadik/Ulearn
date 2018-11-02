export default function getPluralForm(number, one, two, five) {
	number %= 100;
	if (number >= 5 && number <= 20) {
		return five;
	}
	number %= 10;
	if (number === 1) {
		return one;
	}
	if (number >= 2 && number < 5) {
		return two;
	}
	return five;
}

