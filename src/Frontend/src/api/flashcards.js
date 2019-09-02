import api from "./index";

export function getFlashcards(courseId) {
	return api.get(`courses/${ courseId }/flashcards-by-units`);
}

export function putFlashcardStatus(courseId, flashcardId, rate) {
	return api.put(`courses/${ courseId }/flashcards/${ flashcardId }/status`,
		api.createRequestParams(rate));
}
