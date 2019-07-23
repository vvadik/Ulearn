import {
	COURSES__COURSE_ENTERED,
	COURSES__COURSE_LOAD,
	COURSES__FLASHCARDS,
	COURSES__FLASHCARDS_RATE,
	START, SUCCESS, FAIL,
} from "../consts/actions";

import { getCourse } from '../api/courses';
import {
	getFlashcards,
	putFlashcardStatus,
} from '../api/flashcards'

export const changeCurrentCourseAction = (courseId) => ({
	type: COURSES__COURSE_ENTERED,
	courseId,
});

const loadCourseStart = () => ({
	type: COURSES__COURSE_LOAD + START,
});

const loadCourseSuccess = (courseId, result) => ({
	type: COURSES__COURSE_LOAD + SUCCESS,
	courseId,
	result,
});

const loadCourseFail = () => ({
	type: COURSES__COURSE_LOAD + FAIL,
});

const loadFlashcardsStart = () => ({
	type: COURSES__FLASHCARDS + START,
});

const loadFlashcardsSuccess = (courseId, result) => ({
	type: COURSES__FLASHCARDS + SUCCESS,
	courseId,
	result,
});

const loadFlashcardsFail = () => ({
	type: COURSES__FLASHCARDS_RATE + FAIL,
});

const sendFlashcardResultSuccess = (courseId, unitId, flashcardId, rate) => ({
	type: COURSES__FLASHCARDS_RATE + SUCCESS,
	courseId,
	unitId,
	flashcardId,
	rate,
});

export const loadCourse = (courseId) => {
	return (dispatch) => {
		dispatch(loadCourseStart());

		getCourse(courseId)
			.then(result => {
				dispatch(loadCourseSuccess(courseId, result));
			})
			.catch(err => {
				dispatch(loadCourseFail());
			});
	};
};

export const loadFlashcards = (courseId) => {
	return (dispatch) => {
		dispatch(loadFlashcardsStart());

		getFlashcards(courseId)
			.then(result => {
				dispatch(loadFlashcardsSuccess(courseId, result.units));
			})
			.catch(err => {
				dispatch(loadFlashcardsFail());
			});
	}
};

export const sendFlashcardResult = (courseId, unitId, flashcardId, rate) => {
	return (dispatch) => {
		putFlashcardStatus(courseId, flashcardId, rate)
			.then(_ => {
			})
			.catch(err => {
				dispatch(sendFlashcardResultSuccess(courseId, unitId, flashcardId, rate))
			});
	}
};
