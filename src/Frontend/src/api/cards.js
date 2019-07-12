import api from "./index";

export function getUnitFlashcardsStat(courseId, unitId) {
	const query = unitId
		? ''
		: `?unitId=${unitId}`;

	return api.get(`courses/${courseId}/flashcards/stat` + query);
}

export function getFlashcards(courseId, unitId, count = 5, flashcardOrder = 'smart', rate = 'notRated') {
	let query = `?count=${count}&rate=${rate}&flashcardOrder=${flashcardOrder}`;

	if (unitId) {
		query += `&unitId=${unitId}`;
	}

	return api.get(`courses/${courseId}/flashcards${query}`);
}

export function putFlashcardStatus(courseId, cardId, rate) {
	return api.put(`courses/${courseId}/flashcards/${cardId}/status`,
		api.createRequestParams({rate: rate}));
}

export function getCourseFlashcardsStat(courseId) {
	return api.get(`courses/${courseId}/flashcards-info`);
}
