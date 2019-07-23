import api from "./index";
import { buildQuery } from "../utils";

export function getFlashcards(courseId) {
	return api.get(`courses/${ courseId }/flashcards`);
}

export function getFlashcardsStatistics(courseId, unitId) {
	const query = buildQuery({ unitId });

	return api.get(`courses/${ courseId }/flashcards/stat` + query);
}

export function getFlashcardsPack(courseId, unitId, count, flashcardOrder = 'original', rate) {
	let query = buildQuery({
		courseId: courseId,
		unitId: unitId,
		count: count,
		flashcardOrder: flashcardOrder,
		rate: rate
	});

	return api.get(`courses/${ courseId }/flashcards` + query);
}

export function putFlashcardStatus(courseId, flashcardId, rate) {
	return api.put(`courses/${ courseId }/flashcards/${ flashcardId }/status`,
		api.createRequestParams(rate));
}

export function getCourseFlashcardsInfo(courseId) {
	return api.get(`courses/${ courseId }/flashcards-info`);
}
