import {
	COURSES__COURSE_ENTERED,
	COURSES__COURSE_LOAD,
	COURSES__FLASHCARDS_INFO,
	COURSES_FLASHCARDS_PACK,
	START, SUCCESS, FAIL, COURSES_FLASHCARDS_STATISTICS,
} from "../consts/actions";

import {getCourse} from '../api/courses';
import {
	getCourseFlashcardsInfo,
	getFlashcardsPack,
	putFlashcardStatus,
	getFlashcardsStatistics,
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

const loadCourseFlashcardsInfoStart = () => ({
	type: COURSES__FLASHCARDS_INFO + START,
});

const loadCourseFlashcardsInfoSuccess = (courseId, result) => ({
	type: COURSES__FLASHCARDS_INFO + SUCCESS,
	courseId,
	result,
});

const loadCourseFlashcardsInfoFail = () => ({
	type: COURSES__FLASHCARDS_INFO + FAIL,
});

const loadFlashcardsPackStart = () => ({
	type: COURSES_FLASHCARDS_PACK + START,
});

const loadFlashcardsPackSuccess = (courseId, result) => ({
	type: COURSES_FLASHCARDS_PACK + SUCCESS,
	courseId,
	result,
});

const loadFlashcardsPackFail = () => ({
	type: COURSES_FLASHCARDS_PACK + FAIL,
});

const loadFlashcardStatisticsStart = () => ({
	type: COURSES_FLASHCARDS_STATISTICS + START,
});

const loadFlashcardStatisticsSuccess = (courseId, unitId, result) => ({
	type: COURSES_FLASHCARDS_STATISTICS + SUCCESS,
	courseId,
	unitId,
	result,
});

const loadFlashcardStatisticsFail = () => ({
	type: COURSES_FLASHCARDS_STATISTICS + FAIL,
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

export const loadFlashcardsInfo = (courseId) => {
	return (dispatch) => {
		dispatch(loadCourseFlashcardsInfoStart());

		getCourseFlashcardsInfo(courseId)
			.then(result => {
				const flashcardsInfo = result.flashcardsInfo.filter(unit => unit.cardsCount > 0);

				dispatch(loadCourseFlashcardsInfoSuccess(courseId, flashcardsInfo));
			})
			.catch(err => {
				dispatch(loadCourseFlashcardsInfoFail());
			});
	}
};

export const loadFlashcardsPack = (courseId, unitId, count, flashcardOrder, rate) => {
	return (dispatch) => {
		dispatch(loadFlashcardsPackStart());

		getFlashcardsPack(courseId, unitId, count, flashcardOrder, rate)
			.then(result => {
				dispatch(loadFlashcardsPackSuccess(courseId, result.flashcards));
			})
			.catch(err => {
				dispatch(loadFlashcardsPackFail());
			});
	}
};

export const sendFlashcardResult = (courseId, flashcardId, rate) => {
	return (dispatch) => {
		putFlashcardStatus(courseId, flashcardId, rate)
			.catch(err => {
				//dispatch();
			});
	}
};

export const loadStatistics = (courseId, unitId) => {
	return (dispatch) => {
		dispatch(loadFlashcardStatisticsStart());

		getFlashcardsStatistics(courseId, unitId)
			.then(result => {
				dispatch(loadFlashcardStatisticsSuccess(courseId, unitId, result));
			})
			.catch(err => {
				dispatch(loadFlashcardStatisticsFail());
			});
	}
};
