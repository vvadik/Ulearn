import api from "./index";

function buildQuery(params) {
	const esc = encodeURIComponent;
	return '?' + Object
		.keys(params)
		.filter(key => params[key] !== undefined)
		.map(key => esc(key) + '=' + esc(params[key]))
		.join('&');
}

export function getUnitFlashcardsStat(courseId, unitId) {
	const query = buildQuery({unitId});

	return api.get(`courses/${courseId}/flashcards/stat` + query);
}

export function getFlashcards(courseId, unitId, count, flashcardOrder, rate) {
	let query = buildQuery({
		courseId: courseId,
		unitId: unitId,
		count: count,
		flashcardOrder: flashcardOrder,
		rate: rate
	});

	return api.get(`courses/${courseId}/flashcards` + query);
}

export function putFlashcardStatus(courseId, cardId, rate) {
	return api.put(`courses/${courseId}/flashcards/${cardId}/status`,
		api.createRequestParams(rate));
}

export function getCourseFlashcardsStat(courseId) {
	return api.get(`courses/${courseId}/flashcards-info`);
}
