export default function getWordForm(firstWord, secondWord, gender) {
	if (gender === 'female') {
		return firstWord;
	} else {
		return secondWord;
	}
}