export default function getGenderForm(gender: string, female: string, male: string): string {
	if(gender === 'female') {
		return female;
	} else {
		return male;
	}
}
